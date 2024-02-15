namespace RainbowMatchTracker.Models;

[Table("rainbow_team")]
public class RainbowTeam : DbObject
{
    [Column(Unique = true)]
    public required string Name { get; set; }

    [Column(Unique = true)]
    public required string Code { get; set; }

    public string? Url { get; set; }

    public string? Image { get; set; }
}
