namespace RainbowMatchTracker.Database.Services;

using Models;

public interface ITeamDbService : IOrmMap<RainbowTeam>
{
    Task<RainbowTeam[]> Active();

    Task<RainbowTeam[]> ByLeague(Guid id);
}

internal class TeamDbService(IOrmService _orm) : Orm<RainbowTeam>(_orm), ITeamDbService
{
    public Task<RainbowTeam[]> Active()
    {
        const string QUERY = @"WITH teams AS (
    SELECT
        DISTINCT (UNNEST(teams)).id as team_id
    FROM rainbow_match
    WHERE
        status IN (4, 5)
)
SELECT DISTINCT t.*
FROM rainbow_team t 
JOIN teams a ON a.team_id = t.id";
        return Get(QUERY);
    }

    public Task<RainbowTeam[]> ByLeague(Guid id)
    {
        const string QUERY = @"SELECT DISTINCT t.*
FROM rainbow_team t 
JOIN rainbow_match_teams a ON a.team_id = t.id
WHERE
    a.league_id = :id";
        return Get(QUERY, new { id });
    }
}
