namespace RainbowMatchTracker.Services;

using Matches;
using Models;
using Leagues = Dictionary<string, Models.RainbowLeague>;
using Teams = Dictionary<string, Models.RainbowTeam>;

public interface IMatchMergeService
{
    IAsyncEnumerable<MergeResult> Merge();
}

internal class MatchMergeService(
    IDbService _db,
    IMatchApiService _match,
    ILogger<MatchMergeService> _logger) : IMatchMergeService
{
    public async Task<RainbowLeague?> GetLeague(LinkItem? item, Leagues leagues)
    {
        if (item is null ||
            string.IsNullOrEmpty(item.FullName) ||
            string.IsNullOrEmpty(item.Name))
            return null;

        if (leagues.TryGetValue(item.FullName, out var value))
            return value;

        _logger.LogDebug("Could not find existing League, creating it: {FullName} ({Name})", item.FullName, item.Name);

        var league = new RainbowLeague
        {
            Name = item.FullName,
            Display = item.Name,
            Url = item.Url,
            Image = item.ImageUrl,
        };
        league.Id = await _db.League.Upsert(league);
        leagues.Add(league.Name, league);
        return league;
    }

    public async Task<RainbowTeam?> GetTeam(LinkItem? item, Teams teams)
    {
        if (item is null ||
            string.IsNullOrEmpty(item.FullName) ||
            string.IsNullOrEmpty(item.Name))
            return null;

        if (teams.TryGetValue(item.FullName, out var value))
            return value;

        _logger.LogDebug("Could not find existing Team, creating it: {FullName} ({Name})", item.FullName, item.Name);
        var team = new RainbowTeam
        {
            Name = item.FullName,
            Code = item.Name.ToUpper().Trim(),
            Url = item.Url,
            Image = item.ImageUrl,
        };
        team.Id = await _db.Team.Upsert(team);
        teams.Add(team.Name, team);
        return team;
    }

    public static string GetHash(RainbowMatch match)
    {
        var teamIds = match.Teams
            .Select(t => t.Id.ToString())
            .OrderBy(t => t)
            .StrJoin("::");

        return $"{teamIds}::{match.LeagueId}::{match.StartTime:yyyy-MM-dd-HH}".MD5Hash();
    }

    public async Task<RainbowMatch?> GetMatch(Match? match, Leagues leagues, Teams teams)
    {
        if (match is null) return null;

        var leagueTask = GetLeague(match.League, leagues);
        var teamOneTask = GetTeam(match.TeamOne, teams);
        var teamTwoTask = GetTeam(match.TeamTwo, teams);

        await Task.WhenAll(leagueTask, teamOneTask, teamTwoTask);

        var league = leagueTask.Result;
        var teamOne = teamOneTask.Result;
        var teamTwo = teamTwoTask.Result;
        if (league is null || teamOne is null || teamTwo is null) return null;

        var rainbowMatch = new RainbowMatch
        {
            Teams = [
                new RainbowMatchTeam { Id = teamOne.Id, Score = match.TeamOneScore ?? 0 },
                new RainbowMatchTeam { Id = teamTwo.Id, Score = match.TeamTwoScore ?? 0 }
            ],
            LeagueId = league.Id,
            Status = match.Status,
            StartTime = match.StartTime,
            BestOf = match.BestOf,
        };
        rainbowMatch.Hash = GetHash(rainbowMatch);
        return rainbowMatch;
    }

    public static bool ExactMatch(RainbowMatch one, RainbowMatch two)
    {
        return one.Hash == two.Hash &&
            one.StartTime == two.StartTime &&
            one.BestOf == two.BestOf &&
            one.Status == two.Status;
    }

    public async Task<MergeResult> Upsert(RainbowMatch match, DateTime batchTime, MergeStatus status)
    {
        if (status == MergeStatus.Removed)
            match.DeletedAt = DateTime.UtcNow;

        match.LastBatchTime = batchTime;
        match.Id = await _db.Match.Upsert(match);
        return new(match, status);
    }

    public async IAsyncEnumerable<Joined> GetJoinedMatches()
    {
        var (lastBatchMatches, leaguesBatch, teamsBatch) = await _db.Match.LastBatch();
        var lastBatch = lastBatchMatches.ToDictionary(m => m.Hash);
        var leagues = leaguesBatch.ToDictionary(l => l.Name);
        var teams = teamsBatch.ToDictionary(t => t.Name);

        _logger.LogDebug("Starting with {lastBatch} matches, {leagues} leagues, and {teams} teams", lastBatch.Count, leagues.Count, teams.Count);
        await foreach(var match in GetMatches(leagues, teams))
        {
            if (!lastBatch.TryGetValue(match.Hash, out var last))
            {
                _logger.LogDebug("Could not find last match for {Hash}, taking left.", match.Hash);
                yield return new(match, null);
                continue;
            }

            lastBatch.Remove(match.Hash);
            if (ExactMatch(match, last))
            {
                _logger.LogDebug("Match hash is the same, skipping. {Hash}", match.Hash);
                continue;
            }
                
            yield return new(match, last);
            _logger.LogDebug("Differences found: \r\n" +
                "{oneHash} = {twoHash}\r\n" +
                "{oneStart} = {twoStart}\r\n" +
                "{oneBestOf} = {twoBestOf}\r\n" +
                "{oneStatus} = {twoStatus}",
                match.Hash, last.Hash,
                match.StartTime, last.StartTime,
                match.BestOf, last.BestOf,
                match.Status, last.Status);
        }

        foreach (var match in lastBatch.Values)
        {
            _logger.LogDebug("Could not find new match for {Hash}, taking right.", match.Hash);
            yield return new(null, match);
        }
    }

    public async IAsyncEnumerable<RainbowMatch> GetMatches(Leagues leagues, Teams teams)
    {
        var matches = await _match.Matches().ToArrayAsync();
        foreach (var match in matches)
        {
            var rainbowMatch = await GetMatch(match, leagues, teams);
            if (rainbowMatch is not null)
                yield return rainbowMatch;
        }
    }

    public async IAsyncEnumerable<MergeResult> Merge()
    {
        var batchTime = DateTime.UtcNow;

        await foreach (var (newMatch, lastMatch) in GetJoinedMatches())
        {
            //Wtf? Why are both null?
            if (newMatch is null && lastMatch is null) continue;

            //New match, no previous match - insert
            if (newMatch is not null && lastMatch is null)
            {
                yield return await Upsert(newMatch, batchTime, MergeStatus.New);
                continue;
            }

            //Match exists in both - update
            if (newMatch is not null && lastMatch is not null)
            {
                yield return await Upsert(newMatch, batchTime, MergeStatus.Updated);
                continue;
            }

            //Match exists in last batch, but not in new - remove if not upcoming, otherwise update
            yield return await Upsert(lastMatch!, batchTime,
                lastMatch!.Status == MatchStatus.Upcoming
                    ? MergeStatus.Removed
                    : MergeStatus.Updated);
        }
    }
}

internal record class Joined(
    RainbowMatch? NewItem,
    RainbowMatch? LastItem);

public record class MergeResult(
    [property: JsonPropertyName("match")] RainbowMatch Match,
    [property: JsonPropertyName("status")] MergeStatus Status);

public enum MergeStatus
{
    New = 0,
    Removed = 1,
    Updated = 2,
}