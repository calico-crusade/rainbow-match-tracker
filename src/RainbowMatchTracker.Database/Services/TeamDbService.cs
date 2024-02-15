namespace RainbowMatchTracker.Database.Services;

using Models;

public interface ITeamDbService : IOrmMap<RainbowTeam>
{

}

internal class TeamDbService(IOrmService _orm) : Orm<RainbowTeam>(_orm), ITeamDbService
{

}
