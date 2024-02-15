namespace RainbowMatchTracker.Services;

using Matches;
using Models;

public interface IMatchMergeService
{
    IAsyncEnumerable<MergeResult> Merge();
}

internal class MatchMergeService(
    IDbService _db,
    IMatchApiService _match) : IMatchMergeService
{
    public async Task<RainbowLeague?> GetLeague(LinkItem? item)
    {
        if (item is null ||
            string.IsNullOrEmpty(item.FullName) ||
            string.IsNullOrEmpty(item.Name))
            return null;

        var league = new RainbowLeague
        {
            Name = item.FullName,
            Display = item.Name,
            Url = item.Url,
            Image = item.ImageUrl,
        };
        league.Id = await _db.League.Upsert(league);
        return league;
    }

    public async Task<RainbowTeam?> GetTeam(LinkItem? item)
    {
        if (item is null ||
            string.IsNullOrEmpty(item.FullName) ||
            string.IsNullOrEmpty(item.Name))
            return null;

        var team = new RainbowTeam
        {
            Name = item.FullName,
            Code = item.Name,
            Url = item.Url,
            Image = item.ImageUrl,
        };
        team.Id = await _db.Team.Upsert(team);
        return team;
    }

    public static string GetHash(RainbowMatch match)
    {
        return match.Teams
            .Select(t => t.Id.ToString()).OrderBy(t => t)
            .Append(match.LeagueId.ToString())
            .Append(match.StartTime.ToString("yyyy-MM-dd-HH"))
            .StrJoin("::")
            .MD5Hash();
    }

    public async Task<RainbowMatch?> GetMatch(Match? match)
    {
        if (match is null) return null;

        var leagueTask = GetLeague(match.League);
        var teamOneTask = GetTeam(match.TeamOne);
        var teamTwoTask = GetTeam(match.TeamTwo);

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

    public bool ExactMatch(RainbowMatch one, RainbowMatch two)
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
        var lastBatch = (await _db.Match.LastBatch()).ToDictionary(t => t.Hash);

        await foreach(var match in GetMatches())
        {
            if (!lastBatch.TryGetValue(match.Hash, out var last))
            {
                yield return new(match, null);
                continue;
            }

            lastBatch.Remove(match.Hash);
            if (!ExactMatch(match, last))
                yield return new(match, last);
        }

        foreach (var match in lastBatch.Values)
            yield return new(null, match);
    }

    public async IAsyncEnumerable<RainbowMatch> GetMatches()
    {
        var matches = _match.Matches();
        await foreach (var match in matches)
        {
            var rainbowMatch = await GetMatch(match);
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

        //var newMatches = newBatch.Except(lastBatch);
        //var handled = new List<string>();
        //foreach (var match in newMatches)
        //{
        //    yield return await Upsert(match, batchTime, MergeStatus.New);
        //    handled.Add(match.Hash);
        //}

        //var removedMatches = lastBatch.Except(newBatch);
        //foreach (var match in removedMatches)
        //{
        //    yield return await Upsert(match, batchTime,
        //        match.Status == MatchStatus.Upcoming 
        //            ? MergeStatus.Removed
        //            : MergeStatus.Updated);
        //    handled.Add(match.Hash);    
        //}

        //var all = newBatch.Concat(lastBatch);
        //foreach (var match in all)
        //{
        //    if (handled.Contains(match.Hash)) continue;

        //    match.LastBatchTime = batchTime;
        //    match.Id = await _db.Match.Upsert(match);
        //    yield return new(match, MergeStatus.Updated);
        //    handled.Add(match.Hash);
        //}
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