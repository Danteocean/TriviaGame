namespace CoreLibrary.DTOs.Question.Requests;

public class OptionDtoRequest
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}