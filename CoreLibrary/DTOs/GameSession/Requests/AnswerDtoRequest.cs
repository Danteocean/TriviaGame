namespace CoreLibrary.DTOs.GameSession.Requests;

public class AnswerDtoRequest
{
    public Guid SessionId { get; set; }
    public int OptionId { get; set; }
}
