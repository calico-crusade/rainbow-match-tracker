namespace RainbowMatchTracker.Models;

[Type("rainbow_match_team")]
public class RainbowMatchTeam
{
    public required Guid Id { get; set; }

    public required int Score { get; set; }
}