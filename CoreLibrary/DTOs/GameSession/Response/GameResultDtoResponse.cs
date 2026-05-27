namespace CoreLibrary.DTOs.GameSession.Response;

public class GameResultDtoResponse
{
    public bool IsCorrect { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsGameOver { get; set; }
    public int CurrentRound { get; set; }
    public decimal AccumulatedPrize { get; set; }
}
