namespace RainbowMatchTracker;

using Services;

public static class DiExtensions
{
    public static IDependencyResolver AddRollup(this IDependencyResolver resolver)
    {
        return resolver
            .Transient<IMatchMergeService, MatchMergeService>();
    }
}
