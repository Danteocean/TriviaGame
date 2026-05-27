using System.Net.Http.Json;
using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.DTOs.GameSession.Response;
using CoreLibrary.Interface.Services.Client;
using CoreLibrary.Wrappers;

namespace Services.Http;

public class GameSessionHttpClient : IGameSessionHttpClient
{
    private readonly HttpClient _httpClient;
    public GameSessionHttpClient(HttpClient httpClient) => _httpClient = httpClient;

    // Crea la sesi�n en el Backend
    public async Task<Response<Guid>> StartGameAsync(int playerId)
    {
        var res = await _httpClient.PostAsJsonAsync("GameSession/StartGame", playerId);
        return await res.Content.ReadFromJsonAsync<Response<Guid>>()
               ?? new Response<Guid>(Guid.Empty) { Message = "Error al crear sesi�n", Succeeded = false };
    }
    public async Task<Response<GameResultDtoResponse>> SubmitAnswerAsync(AnswerDtoRequest request)
    {
        var res = await _httpClient.PostAsJsonAsync("GameSession/SubmitAnswer", request);
        return await res.Content.ReadFromJsonAsync<Response<GameResultDtoResponse>>()
               ?? new Response<GameResultDtoResponse>(null!);
    }

    public async Task<Response<GameResultDtoResponse>> GetSessionByIdAsync(Guid sessionId)
    {
        var res = await _httpClient.GetAsync($"GameSession/GetById/{sessionId}");
        return await res.Content.ReadFromJsonAsync<Response<GameResultDtoResponse>>()
               ?? new Response<GameResultDtoResponse>(null!);
    }

    public async Task<Response<bool>> WithdrawAsync(Guid sessionId)
    {
        var res = await _httpClient.PostAsJsonAsync("GameSession/Withdraw", sessionId);
        return await res.Content.ReadFromJsonAsync<Response<bool>>() ?? new Response<bool>(false);
    }

    public async Task<Response<bool>> EndGameAsync(EndGameRequest request)
    {
        var res = await _httpClient.PostAsJsonAsync("GameSession/EndGame", request);
        return await res.Content.ReadFromJsonAsync<Response<bool>>() ?? new Response<bool>(false);
    }

    public async Task<Response<QuestionDtoResponse>> GetNextQuestionAsync(Guid sessionId)
    {
        var res = await _httpClient.GetAsync($"GameSession/GetNextQuestion/{sessionId}");
        return await res.Content.ReadFromJsonAsync<Response<QuestionDtoResponse>>() ?? new Response<QuestionDtoResponse>(null!);
    }
}