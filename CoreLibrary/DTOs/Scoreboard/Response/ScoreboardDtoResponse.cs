namespace CoreLibrary.DTOs.Scoreboard.Response;

public class ScoreboardDtoResponse
{
    public string Alias { get; set; } = string.Empty;
    public decimal AccumulatedPrize { get; set; }
    public DateTime CreatedAt { get; set; }
}