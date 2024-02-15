namespace RainbowMatchTracker.Database.Services;

using Models;

public interface IMatchDbService : IOrmMap<RainbowMatch>
{
    Task<RainbowMatch[]> LastBatch();
}

internal class MatchDbService(IOrmService _orm) : Orm<RainbowMatch>(_orm), IMatchDbService
{
    public Task<RainbowMatch[]> LastBatch()
    {
        const string QUERY = @"; WITH last_batch AS (
    SELECT 
        MAX(last_batch_time) as last_batch_time 
    FROM rainbow_match
    WHERE deleted_at IS NULL
)
SELECT * 
FROM rainbow_match m
JOIN last_batch b ON b.last_batch_time = m.last_batch_time
WHERE m.deleted_at IS NULL";
        return Get(QUERY);
    }
}
