namespace RainbowMatchTracker.Models;

public class ExtendedMatch
{
    public required RainbowMatch Match { get; set; }

    public required RainbowLeague League { get; set; }

    public required RainbowTeam[] Teams { get; set; }
}
