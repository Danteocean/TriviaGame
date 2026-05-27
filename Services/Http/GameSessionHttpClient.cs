using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.DTOs.GameSession.Response;
using CoreLibrary.Wrappers;
using Services.Common;

namespace Services.Http;

public class GameSessionHttpClient : BaseHttpClient, IGameSessionHttpClient
{
    public GameSessionHttpClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<Response<Guid>> StartGameAsync(int playerId)
    {
        return await SendPostAsync<int, Guid>("GameSession/StartGame", playerId);
    }

    public async Task<Response<QuestionDtoResponse>> GetNextQuestionAsync(Guid sessionId)
    {
        return await SendGetAsync<QuestionDtoResponse>($"GameSession/GetNextQuestion/{sessionId}");
    }

    public async Task<Response<GameResultDtoResponse>> SubmitAnswerAsync(AnswerDtoRequest request)
    {
        return await SendPostAsync<AnswerDtoRequest, GameResultDtoResponse>("GameSession/SubmitAnswer", request);
    }
}
