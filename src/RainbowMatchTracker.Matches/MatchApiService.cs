using HtmlAgilityPack;

namespace RainbowMatchTracker.Matches;

using Models;

public interface IMatchApiService
{
    /// <summary>
    /// Returns a collection of matches using the <see cref="MatchApiService.MATCHES_URL"/> endpoint
    /// </summary>
    /// <returns>A collection of R6 matches</returns>
    IAsyncEnumerable<Match> Matches();

    /// <summary>
    /// Returns a collection of matches using the given url
    /// </summary>
    /// <param name="url">The URL to pull the results from</param>
    /// <returns>A collection of R6 matches</returns>
    IAsyncEnumerable<Match> Matches(string url);
}

internal class MatchApiService(
    ILogger<MatchApiService> _logger,
    IConfiguration _config) : IMatchApiService
{
    /// <summary>
    /// Base Url for the r6 matches API
    /// </summary>
    public string BaseUrl => _config["Matches:Url"] ?? throw new ArgumentNullException("Matches:Url");

    /// <summary>
    /// The matches endpoint for the r6 matches API
    /// </summary>
    public string Endpoint => _config["Matches:Endpoint"] ?? throw new ArgumentNullException("Matches:Endpoint");

    /// <summary>
    /// Returns a collection of matches using the <see cref="MATCHES_URL"/> endpoint
    /// </summary>
    /// <returns>A collection of R6 matches</returns>
    public IAsyncEnumerable<Match> Matches() => Matches($"{BaseUrl}/{Endpoint}");

    /// <summary>
    /// Returns a collection of matches using the given url
    /// </summary>
    /// <param name="url">The URL to pull the results from</param>
    /// <returns>A collection of R6 matches</returns>
    public async IAsyncEnumerable<Match> Matches(string url)
    {
        var document = await LoadFromUrl(url);

        var tables = document.DocumentNode.SelectNodes("//table");
        if (tables == null) yield break;

        foreach(var match in Matches(tables))
            yield return match;
    }

    /// <summary>
    /// Iterates through the given table node collection
    /// </summary>
    /// <param name="tables">The table nodes to iterate through</param>
    /// <returns>All of the matches</returns>
    public IEnumerable<Match> Matches(HtmlNodeCollection tables)
    {
        foreach (var table in tables)
        {
            //Only get table values that have the class infobox_matches_content
            if (!table.Attributes["class"].Value.Contains("infobox_matches_content"))
                continue;

            //Fetch a new document from the given tables HTML
            var singleDoc = LoadFromString(table.InnerHtml);

            //Setup the match
            var match = new Match
            {
                TeamOne = GetTeam(singleDoc),
                TeamTwo = GetTeam(singleDoc, false),
                League = League(singleDoc),
                StartTime = GetTimestamp(singleDoc)
            };
            AddScore(singleDoc, match);
            match.Status = DetermineState(match);

            yield return match;
        }
    }

    /// <summary>
    /// Determines the match status
    /// </summary>
    /// <param name="match">The match to check</param>
    /// <returns>The match status</returns>
    public static MatchStatus DetermineState(Match match)
    {
        if (!match.TeamOneScore.HasValue || !match.TeamTwoScore.HasValue)
        {
            var now = DateTime.UtcNow;
            var local = match.StartTime;
            if (local > now)
                return MatchStatus.Upcoming;

            if (local <= now && local > now.AddMinutes(-120))
                return MatchStatus.Active;

            return MatchStatus.Unknown;
        }

        var t1 = match.TeamOneScore ?? 0;
        var t2 = match.TeamTwoScore ?? 0;

        if (match.BestOf > 1 && t1 + t2 < (match.BestOf / 2))
            return MatchStatus.Active;

        if (t1 > t2)
            return MatchStatus.TeamOneWon;

        if (t1 < t2)
            return MatchStatus.TeamTwoWon;

        return MatchStatus.Draw;
    }

    /// <summary>
    /// Fetches the offset for the given match
    /// </summary>
    /// <param name="document">The match HTML</param>
    /// <returns>The Start time</returns>
    public static DateTime GetTimestamp(HtmlDocument document)
    {
        var start = "//td[@class='match-filler']/span[@class='match-countdown']/span";

        var item = document.DocumentNode.SelectSingleNode(start);

        var time = item?.Attributes["data-timestamp"]?.Value ?? "0";
        return DateTimeOffset.FromUnixTimeSeconds(long.Parse(time)).UtcDateTime;
    }

    /// <summary>
    /// Fetches the score for the given match
    /// </summary>
    /// <param name="document">The match HTML</param>
    /// <param name="match">The match to populate</param>
    public void AddScore(HtmlDocument document, Match match)
    {
        var start = "//td[@class='versus']";

        var text = document.DocumentNode.SelectSingleNode(start).InnerText.Trim();
        if (text.Contains("(bo", StringComparison.CurrentCultureIgnoreCase))
        {
            var parts = text.ToLower().Split(["(bo"], StringSplitOptions.RemoveEmptyEntries);
            var num = parts[1].Split(')')[0];

            text = text
                .Replace("(Bo" + num + ")", "")
                .Replace("(bo" + num + ")", "")
                .Replace("(BO" + num + ")", "");

            if (int.TryParse(num, out int bestOf))
                match.BestOf = bestOf;
            else
                match.BestOf = 1;
        }

        if (text == "vs." ||
            text == "vs")
        {
            match.TeamOneScore = null;
            match.TeamTwoScore = null;
            return;
        }

        var args = text.Split(':');
        if (args.Length != 2)
        {
            _logger.LogWarning("Invalid score: {text}", text);
            return;
        }


        if (!int.TryParse(args[0], out int s1))
        {
            _logger.LogWarning("Invalid Team One score: {args}", args[0]);
            return;
        }

        if (!int.TryParse(args[1], out int s2))
        {
            _logger.LogWarning("Invalid Team Two score: {args}", args[1]);
            return;
        }

        match.TeamOneScore = s1;
        match.TeamTwoScore = s2;
    }

    /// <summary>
    /// Returns information about the league from the HTML document
    /// </summary>
    /// <param name="document">The document to parse</param>
    /// <returns>The league item</returns>
    public LinkItem? League(HtmlDocument document)
    {
        try
        {
            var imageLinkStr = "//td[@class='match-filler']/div/span/span/a/img";
            var image = document.DocumentNode.SelectSingleNode(imageLinkStr);

            var linkStr = "//td[@class='match-filler']/div/div/a";
            var link = document.DocumentNode.SelectSingleNode(linkStr);

            var li = new LinkItem();

            if (image != null)
                foreach (var attribute in image.Attributes)
                    if (attribute.Name == "src")
                        li.ImageUrl = AppendLink(attribute.Value.Trim());

            li.Url = AppendLink(link.Attributes["href"].Value.Trim());
            li.FullName = link.Attributes["title"].Value.Trim();
            li.Name = link.InnerText.Trim();

            return li;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse league");
            return null;
        }
    }

    /// <summary>
    /// Returns the information about the team from the HTML document
    /// </summary>
    /// <param name="document">The document to parse</param>
    /// <param name="teamOne">Whether or not we're parsing for team One or Two</param>
    /// <returns>The team item</returns>
    public LinkItem GetTeam(HtmlDocument document, bool teamOne = true)
    {
        var start = teamOne ? "//td[@class='team-left']" : "//td[@class='team-right']";
        return new LinkItem
        {
            Url = AppendLink(document.DocumentNode.SelectSingleNode(start + "/span/span[@class='team-template-text']/a")?.Attributes["href"]?.Value?.Trim()),
            FullName = document.DocumentNode.SelectSingleNode(start + "/span/span[@class='team-template-text']/a")?.Attributes["title"]?.Value?.Replace("(page does not exist)", "")?.Trim(),
            Name = document.DocumentNode.SelectSingleNode(start)?.InnerText?.Trim(),
            ImageUrl = AppendLink(document.DocumentNode.SelectSingleNode(start + "/span/span[@class='team-template-image-icon team-template-lightmode']/a/img")?.Attributes["src"]?.Value?.Trim())
        };
    }

    /// <summary>
    /// prepends the <see cref="TournamentApi.BASE_URL"/> to any links
    /// </summary>
    /// <param name="url">The url to prepend to</param>
    /// <returns>The formatted link</returns>
    public string? AppendLink(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        if (url.Contains("://"))
            return url;

        return $"{BaseUrl.Trim('/')}/{url.Trim('/')}";
    }

    /// <summary>
    /// Load the HTML document from the given URL
    /// </summary>
    /// <param name="url">The URL to load from</param>
    /// <returns>The HTML document</returns>
    public static Task<HtmlDocument> LoadFromUrl(string url)
    {
        return new HtmlWeb().LoadFromWebAsync(url);
    }

    /// <summary>
    /// Loads the HTML document from the given HTML string
    /// </summary>
    /// <param name="html">The HTML to load from</param>
    /// <returns>The HTML document</returns>
    public static HtmlDocument LoadFromString(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }
}
