namespace RainbowMatchTracker.Models;

public class MatchSearchRequest
{
    [JsonPropertyName("league")]
    public Guid? League { get; set; }

    [JsonPropertyName("codes")]
    public string[]? Codes { get; set; }

    [JsonPropertyName("ids")]
    public Guid[]? Ids { get; set; }

    [JsonPropertyName("statuses")]
    public MatchStatus[]? Statuses { get; set; }
}
