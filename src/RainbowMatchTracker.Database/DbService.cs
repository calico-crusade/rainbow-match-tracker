namespace RainbowMatchTracker;

using Database.Services;

public interface IDbService
{
    ITeamDbService Team { get; }
    ILeagueDbService League { get; }
    IMatchDbService Match { get; }
}

internal class DbService(
    ITeamDbService team,
    ILeagueDbService league,
    IMatchDbService match) : IDbService
{
    public ITeamDbService Team => team;
    public ILeagueDbService League => league;
    public IMatchDbService Match => match;
}
