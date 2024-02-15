namespace RainbowMatchTracker.Api.Controllers;

using Models;
using Services;

public class MatchesController(
    IMatchMergeService _matches,
    IDbService _db) : BaseController
{
    [HttpGet, Route("api/v1/matches/all")]
    [ResultsIn<RainbowMatch[]>]
    public async Task<IActionResult> Get()
    {
        var matches = await _db.Match.Get();
        return DoOk(matches);
    }

    [HttpGet, Route("api/v1/matches/merge")]
    [ResultsIn<MergeResult[]>]
    public async Task<IActionResult> All()
    {
        var matches = await _matches.Merge().ToArrayAsync();
        return DoOk(matches);
    }
}
