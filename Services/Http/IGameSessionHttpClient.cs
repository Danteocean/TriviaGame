using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.DTOs.GameSession.Response;
using CoreLibrary.Wrappers;

namespace Services.Http;

public interface IGameSessionHttpClient
{
    Task<Response<Guid>> StartGameAsync(int playerId);
    Task<Response<QuestionDtoResponse>> GetNextQuestionAsync(Guid sessionId);
    Task<Response<GameResultDtoResponse>> SubmitAnswerAsync(AnswerDtoRequest request);
}
