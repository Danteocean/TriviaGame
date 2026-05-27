using CoreLibrary.DTOs.Player.Requests;
using CoreLibrary.DTOs.Player.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface IPlayerService
{
    Task<Response<int>> RegisterPlayerAsync(PlayerDtoRequest request);
    Task<Response<PlayerDtoResponse>> GetPlayerByIdAsync(int id);

    Task<Response<PlayerDtoResponse>> GetOrCreatePlayerByAliasAsync(string alias);
}
