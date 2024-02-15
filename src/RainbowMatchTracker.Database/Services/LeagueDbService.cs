namespace RainbowMatchTracker.Database.Services;

using Models;

public interface ILeagueDbService : IOrmMap<RainbowLeague>
{
    Task<RainbowLeague[]> Upsert(RainbowLeague[] leagues);
}

internal class LeagueDbService(IOrmService _orm) : Orm<RainbowLeague>(_orm), ILeagueDbService
{
    public Task<RainbowLeague[]> Upsert(RainbowLeague[] leagues)
    {
        const string QUERY = @"WITH new_league AS (
    SELECT
         a.Name as name,
         a.Display as display,
         a.Url as url,
         a.Image as image,
         a.DeletedAt as deleted_at
    FROM unnest(:Updates) a
), updated AS (
    INSERT INTO rainbow_league (name, display, url, image, deleted_at)
    SELECT
        a.name,
        a.display,
        a.url,
        a.image,
        a.deleted_at
    FROM new_league a
    ON CONFLICT  (name, display)
    DO UPDATE SET
        name = a.name,
        display = a.display,
        url = a.url,
        image = a.image,
        updated_at = CURRENT_TIMESTAMP,
        deleted_at = a.deleted_at
    RETURNING id
)
SELECT a.*
FROM rainbow_league a
JOIN updated b ON b.id = a.id";
        return Get(QUERY, new { Updates = leagues });
    }
}
