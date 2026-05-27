using CoreLibrary.DTOs.Player.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services.Client;

public interface IPlayerHttpClient
{
    Task<Response<PlayerDtoResponse>> EnterGameAsync(string alias);
    Task<Response<PlayerDtoResponse>> GetPlayerByIdAsync(int id);
}
