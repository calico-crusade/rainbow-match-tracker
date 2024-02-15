namespace RainbowMatchTracker;

using Matches;

public static class DiExtensions
{
    public static IDependencyResolver AddMatches(this IDependencyResolver resolver)
    {
        return resolver
            .Transient<IMatchApiService, MatchApiService>();
    }
}
