namespace RainbowMatchTracker.Database.Services;

using Models;

public interface ITeamDbService : IOrmMap<RainbowTeam>
{
    Task<RainbowTeam[]> Active();
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
}
