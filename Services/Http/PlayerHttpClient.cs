using CoreLibrary.DTOs.Player.Response;
using CoreLibrary.Interface.Services.Client;
using CoreLibrary.Wrappers;
using Services.Common;
using System.Net.Http.Json;

namespace Services.Http;

public class PlayerHttpClient : BaseHttpClient, IPlayerHttpClient
{
    public PlayerHttpClient(HttpClient httpClient) : base(httpClient) { }

    public async Task<Response<PlayerDtoResponse>> EnterGameAsync(string alias)
    {
     
        var response = await _httpClient.PostAsJsonAsync("Player/EnterGame", alias);

        if (!response.IsSuccessStatusCode)
            return new Response<PlayerDtoResponse>(null!) { Message = "Error al conectar con el servidor", Succeeded = false };

        return await response.Content.ReadFromJsonAsync<Response<PlayerDtoResponse>>()
               ?? new Response<PlayerDtoResponse>(null!) { Message = "Error al deserializar", Succeeded = false };
    }

    public async Task<Response<PlayerDtoResponse>> GetPlayerByIdAsync(int id)
    {
        return await SendGetAsync<PlayerDtoResponse>($"Player/GetById/{id}");
    }
}