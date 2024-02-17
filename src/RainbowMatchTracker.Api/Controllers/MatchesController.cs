namespace RainbowMatchTracker.Api.Controllers;

public class MatchesController(
    IDbService _db) : BaseController
{
    [HttpGet, Route("matches"), ResultsIn<PaginatedResult<RainbowMatch>>]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var matches = await _db.Match.Paginate(page, size);
        return DoOk(matches);
    }

    [HttpGet, Route("matches/{id}"), ResultsIn<ExtendedMatch>]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var match = await _db.Match.FetchExt(id);
        return match is null 
            ? DoNotFound("Match")
            : DoOk(match);
    }

    [HttpGet, Route("matches/range"), ResultsIn<RainbowMatch[]>]
    public async Task<IActionResult> Range([FromQuery] DateTime start, [FromQuery] DateTime? end = null)
    {
        var matches = await _db.Match.Range(start, end ?? DateTime.UtcNow.AddDays(14));
        return DoOk(matches);
    }

    [HttpPost, Route("matches/search"), ResultsIn<RainbowMatch[]>]
    public async Task<IActionResult> Search([FromBody] MatchSearchRequest request)
    {
        var matches = await _db.Match.Search(request);
        return DoOk(matches);
    }
}
