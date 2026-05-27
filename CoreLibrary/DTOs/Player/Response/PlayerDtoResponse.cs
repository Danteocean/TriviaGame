namespace CoreLibrary.DTOs.Player.Response;

public class PlayerDtoResponse
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public int TotalPointsAchieved { get; set; }
}