namespace CoreLibrary.DTOs.Question.Requests;

public class QuestionDtoRequest
{
    public string Text { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public List<OptionDtoRequest> Options { get; set; } = new();
}