namespace RainbowMatchTracker.Matches;

using Models;

/// <summary>
/// Represents a R6 match
/// </summary>
public class Match
{
    /// <summary>
    /// The first teams details
    /// </summary>
    public required LinkItem TeamOne { get; set; }

    /// <summary>
    /// The second teams details
    /// </summary>
    public required LinkItem TeamTwo { get; set; }

    /// <summary>
    /// The leagues details
    /// </summary>
    public LinkItem? League { get; set; }

    /// <summary>
    /// The server's best guess at the status of the match
    /// </summary>
    public MatchStatus Status { get; set; }

    /// <summary>
    /// The first teams score, or null if they don't have one yet
    /// </summary>
    public int? TeamOneScore { get; set; }

    /// <summary>
    /// The second teams score, or null if they don't have one yet
    /// </summary>
    public int? TeamTwoScore { get; set; }

    /// <summary>
    /// The UTC date time for the matches start time
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The number of maps / games in this match
    /// </summary>
    public int BestOf { get; set; } = 1;

    public override string ToString()
    {
        return $"{TeamOne}\r\nVS\r\n{TeamTwo}\r\n{StartTime}\r\n{League}\r\n{TeamOneScore}:{TeamTwoScore}";
    }
}
