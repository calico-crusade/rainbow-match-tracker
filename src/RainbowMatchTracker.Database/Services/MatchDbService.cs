namespace RainbowMatchTracker.Database.Services;

using Models;

public interface IMatchDbService : IOrmMap<RainbowMatch>
{
    Task<(RainbowMatch[], RainbowLeague[], RainbowTeam[])> LastBatch();

    Task<PaginatedResult<RainbowMatch>> ByLeague(Guid leagueId, int page, int size, params MatchStatus[] status);

    Task<PaginatedResult<RainbowMatch>> ByTeam(Guid teamId, int page, int size);

    Task<ExtendedMatch?> FetchExt(Guid id);

    Task<RainbowMatch[]> Range(DateTime start, DateTime end);

    Task<RainbowMatch[]> ActiveByLeague(Guid leagueId);
}

internal class MatchDbService(IOrmService _orm) : Orm<RainbowMatch>(_orm), IMatchDbService
{
    private static string? _allMatches;
    private static MatchStatus[] ALL_STATUS = [
        MatchStatus.TeamOneWon,
        MatchStatus.TeamTwoWon,
        MatchStatus.Draw,
        MatchStatus.Active,
        MatchStatus.Upcoming,
        MatchStatus.Unknown
    ];

    public override Task<PaginatedResult<RainbowMatch>> Paginate(int page = 1, int size = 100)
    {
        _allMatches ??= Map.Paginate(t => t.StartTime, false);
        return Paginate(_allMatches, null, page, size);
    }

    public async Task<(RainbowMatch[], RainbowLeague[], RainbowTeam[])> LastBatch()
    {
        const string QUERY = @"
WITH last_date AS (
    SELECT CURRENT_DATE - INTERVAL '3 days' AS last_batch_time
)
SELECT DISTINCT m.*
FROM rainbow_match m
JOIN last_date ld ON m.last_batch_time > ld.last_batch_time
WHERE 
    m.deleted_at IS NULL;

SELECT DISTINCT l.*
FROM rainbow_league l
WHERE l.deleted_at IS NULL;

SELECT DISTINCT t.*
FROM rainbow_team t
WHERE t.deleted_at IS NULL;";
        using var con = await _sql.CreateConnection();
        using var rdr = await con.QueryMultipleAsync(QUERY);

        var matches = (await rdr.ReadAsync<RainbowMatch>()).ToArray();
        var leagues = (await rdr.ReadAsync<RainbowLeague>()).ToArray();
        var teams = (await rdr.ReadAsync<RainbowTeam>()).ToArray();
        return (matches, leagues, teams);
    }

    public Task<PaginatedResult<RainbowMatch>> ByTeam(Guid teamId, int page, int size)
    {
        const string QUERY = @"
SELECT
    DISTINCT *
FROM rainbow_match
WHERE
    :teamId = ANY(
        SELECT (UNNEST(teams)).id
    ) AND
    deleted_at IS NULL
ORDER BY start_time DESC
LIMIT :limit
OFFSET :offset;

SELECT COUNT(DISTINCT id)
FROM rainbow_match
WHERE
    :teamId = ANY(
        SELECT (UNNEST(teams)).id
    ) AND
    deleted_at IS NULL;";
        return Paginate(QUERY, new { teamId }, page, size);
    }

    public async Task<ExtendedMatch?> FetchExt(Guid id)
    {
        const string QUERY = @"SELECT * 
FROM rainbow_match 
WHERE 
    id = :id AND
    deleted_at IS NULL;

SELECT DISTINCT l.* 
FROM rainbow_match m 
JOIN rainbow_league l ON m.league_id = l.id
WHERE
    m.id = :id AND
    m.deleted_at IS NULL AND
    l.deleted_at IS NULL;

WITH teams AS (
    SELECT
        (UNNEST(teams)).id as team_id
    FROM rainbow_match 
    WHERE 
        id = :id AND 
        deleted_at IS NULL
)
SELECT 
    DISTINCT t.*
FROM rainbow_team t 
JOIN teams a ON a.team_id = t.id
WHERE t.deleted_at IS NULL;";

        using var con = await _sql.CreateConnection();
        using var rdr = await con.QueryMultipleAsync(QUERY, new { id });

        var match = await rdr.ReadFirstOrDefaultAsync<RainbowMatch>();
        if (match is null) return null;

        var league = await rdr.ReadFirstOrDefaultAsync<RainbowLeague>();
        var teams = (await rdr.ReadAsync<RainbowTeam>()).ToArray();

        return new ExtendedMatch
        {
            Match = match,
            League = league,
            Teams = teams
        };
    }

    public Task<RainbowMatch[]> Range(DateTime start, DateTime end)
    {
        const string QUERY = @"SELECT *
FROM rainbow_match
WHERE 
    start_time BETWEEN :start AND :end AND
    deleted_at IS NULL;";
        return Get(QUERY, new { start, end });
    }

    public Task<RainbowMatch[]> ActiveByLeague(Guid leagueId)
    {
        const string QUERY = @"SELECT * 
FROM rainbow_match
WHERE 
    league_id = :leagueId AND
    status IN (4, 5) AND
    deleted_at IS NULL
ORDER BY start_time ASC";
        return Get(QUERY, new { leagueId });
    }

    public Task<PaginatedResult<RainbowMatch>> ByLeague(Guid leagueId, int page, int size, params MatchStatus[] status)
    {
        const string QUERY = @"
SELECT * 
FROM rainbow_match
WHERE 
    league_id = :leagueId AND 
    deleted_at IS NULL AND
    status = ANY(:statuses)
ORDER BY start_time DESC 
LIMIT :limit 
OFFSET :offset; 

SELECT COUNT(*) 
FROM rainbow_match
WHERE  
    league_id = :leagueId AND 
    deleted_at IS NULL AND
    status = ANY(:statuses);";

        var statuses = (status.Length == 0 ? ALL_STATUS : status).Select(t => (int)t).ToArray();

        return Paginate(QUERY, new { leagueId, statuses }, page, size);
    }
}
