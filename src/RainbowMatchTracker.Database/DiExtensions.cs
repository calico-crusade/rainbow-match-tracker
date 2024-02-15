namespace RainbowMatchTracker;

using Database.Services;
using Models;

public static class  DiExtensions
{
    public static IDependencyResolver AddDatabase(this IDependencyResolver resolver)
    {
        return resolver

            .Transient<ITeamDbService, TeamDbService>()
            .Transient<ILeagueDbService, LeagueDbService>()
            .Transient<IMatchDbService, MatchDbService>()

            .Transient<IDbService, DbService>()
            
            .Type<RainbowMatchTeam>("rainbow_match_team");
    }
}
