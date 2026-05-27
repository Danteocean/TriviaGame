namespace CoreLibrary.DTOs.Question.Response;

public class QuestionDtoResponse
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<OptionDtoResponse> Options { get; set; } = new();
}

public class OptionDtoResponse
{
    public int OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}