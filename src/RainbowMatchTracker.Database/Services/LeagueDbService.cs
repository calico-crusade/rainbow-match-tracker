namespace RainbowMatchTracker.Database.Services;

using Models;

public interface ILeagueDbService : IOrmMap<RainbowLeague>
{
    Task<RainbowLeague[]> Active();
}

internal class LeagueDbService(IOrmService _orm) : Orm<RainbowLeague>(_orm), ILeagueDbService
{
    public Task<RainbowLeague[]> Active()
    {
        const string QUERY = @"SELECT
    DISTINCT l.*
FROM rainbow_league l
JOIN rainbow_match m ON l.id = m.league_id
WHERE
    m.status IN (4, 5) AND
    l.deleted_at IS NULL AND
    m.deleted_at IS NULL;";

        return Get(QUERY);
    }
}