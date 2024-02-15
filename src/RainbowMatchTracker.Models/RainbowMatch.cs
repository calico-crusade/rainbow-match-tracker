namespace RainbowMatchTracker.Models;

[Table("rainbow_match")]
public class RainbowMatch : DbObject, IEquatable<RainbowMatch>
{
    [Column(Unique = true)]
    public string Hash { get; set; } = string.Empty;

    public required RainbowMatchTeam[] Teams { get; set; }

    [Column("league_id")]
    public required Guid LeagueId { get; set; }

    public required MatchStatus Status { get; set; }

    [Column("start_time")]
    public DateTime StartTime { get; set; }

    [Column("best_of")]
    public int BestOf { get; set; }

    [Column("last_batch_time")]
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