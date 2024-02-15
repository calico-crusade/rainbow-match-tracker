namespace RainbowMatchTracker.Api.Controllers;

public class TeamController(IDbService _db): BaseController
{
    [HttpGet, Route("api/v1/teams/active"), ResultsIn<RainbowTeam[]>]
    public async Task<IActionResult> Active()
    {
        var items = await _db.Team.Active();
        return DoOk(items);
    }

    [HttpGet, Route("api/v1/teams"), ResultsIn<PaginatedResult<RainbowTeam>>]
    public async Task<IActionResult> Paginate([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var items = await _db.Team.Paginate(page, size);
        return DoOk(items);
    }

    [HttpGet, Route("api/v1/teams/{id}")]
    [ResultsIn<RainbowTeam>, ResultsInError(404)]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.Team.Fetch(id);
        return item is null
            ? DoNotFound("Team")
            : DoOk(item);
    }

    [HttpGet, Route("api/v1/teams/{id}/matches"), ResultsIn<PaginatedResult<RainbowTeam>>]
    public async Task<IActionResult> Matches(Guid id, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var matches = await _db.Match.ByTeam(id, page, size);
        return DoOk(matches);
    }
}
