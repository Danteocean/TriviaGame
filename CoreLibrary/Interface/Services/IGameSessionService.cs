using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.DTOs.GameSession.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface IGameSessionService
{
    Task<Response<GameResultDtoResponse>> GetByIdAsync(Guid sessionId);
    Task<Response<Guid>> StartGameAsync(int playerId);
    Task<Response<QuestionDtoResponse>> GetNextQuestionAsync(Guid sessionId);
    Task<Response<GameResultDtoResponse>> SubmitAnswerAsync(AnswerDtoRequest request);
    Task<Response<bool>> WithdrawAsync(Guid sessionId);

    Task<Response<bool>> EndGameAsync(EndGameRequest request);
}