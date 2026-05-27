public class EndGameRequest
{
    public Guid GameSessionId { get; set; }
    public int FinalScore { get; set; }
    public bool DidRetire { get; set; }
}