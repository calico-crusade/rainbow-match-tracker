namespace RainbowMatchTracker.Models;

[Table("rainbow_league")]
public class RainbowLeague : DbObject
{
    [Column(Unique = true)]
    public required string Name { get; set; }

    [Column(Unique = true)]
    public required string Display { get; set; }

    public string? Url { get; set; }

    public string? Image { get; set; }
}