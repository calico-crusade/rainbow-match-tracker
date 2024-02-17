namespace RainbowMatchTracker.Api.Controllers;

public class LeagueController(IDbService _db) : BaseController
{
    [HttpGet, Route("leagues/active"), ResultsIn<RainbowLeague[]>]
    public async Task<IActionResult> Active()
    {
        var leagues = await _db.League.Active();
        return DoOk(leagues);
    }

    [HttpGet, Route("leagues"), ResultsIn<PaginatedResult<RainbowLeague>>]
    public async Task<IActionResult> Paginate([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var leagues = await _db.League.Paginate(page, size);
        return DoOk(leagues);
    }

    [HttpGet, Route("leagues/{id}")]
    [ResultsIn<RainbowLeague>, ResultsInError(404)]
    public async Task<IActionResult> Get(Guid id)
    {
        var league = await _db.League.Fetch(id);
        return league is null 
            ? DoNotFound("League")
            : DoOk(league);
    }

    [HttpGet, Route("leagues/{id}/teams"), ResultsIn<RainbowTeam[]>]
    public async Task<IActionResult> Teams(Guid id)
    {
        var teams = await _db.Team.ByLeague(id);
        return DoOk(teams);
    }

    [HttpGet, Route("leagues/{id}/matches"), ResultsIn<PaginatedResult<RainbowMatch>>]
    public async Task<IActionResult> Matches(Guid id, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var matches = await _db.Match.ByLeague(id, page, size);
        return DoOk(matches);
    }

    [HttpGet, Route("leagues/{id}/matches/finished"), ResultsIn<PaginatedResult<RainbowMatch>>]
    public async Task<IActionResult> FinishedMatches(Guid id, [FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var matches = await _db.Match.ByLeague(id, page, size, 
            MatchStatus.TeamOneWon, MatchStatus.TeamTwoWon, MatchStatus.Draw);
        return DoOk(matches);
    }

    [HttpGet, Route("leagues/{id}/matches/active"), ResultsIn<RainbowMatch[]>]
    public async Task<IActionResult> ActiveMatches(Guid id)
    {
        var matches = await _db.Match.ActiveByLeague(id);
        return DoOk(matches);
    }
}
