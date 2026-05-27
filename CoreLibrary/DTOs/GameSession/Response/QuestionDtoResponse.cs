namespace CoreLibrary.DTOs.GameSession.Response;

public class QuestionDtoResponse
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<OptionDtoResponse> Options { get; set; } = new();
}

public class OptionDtoResponse
{
    public int OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
}