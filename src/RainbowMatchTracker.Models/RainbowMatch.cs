namespace RainbowMatchTracker.Models;

[Table("rainbow_match")]
public class RainbowMatch : DbObject, IEquatable<RainbowMatch>
{
    [Column(Unique = true)]
    public string Hash { get; set; } = string.Empty;

    public required RainbowMatchTeam[] Teams { get; set; }

    public required Guid LeagueId { get; set; }

    public required MatchStatus Status { get; set; }

    public DateTime StartTime { get; set; }

    public int BestOf { get; set; }

    public DateTime LastBatchTime { get; set; }

    public bool Equals(RainbowMatch? other)
    {
        if (other is null) return false;
        return Hash == other.Hash;
    }
    public override bool Equals(object? obj)
    {
        return Equals(obj as RainbowMatch);
    }

    public override int GetHashCode()
    {
        return Hash.GetHashCode();
    }
}