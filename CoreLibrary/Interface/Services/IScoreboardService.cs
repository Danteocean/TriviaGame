using CoreLibrary.DTOs.Scoreboard.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface IScoreboardService
{
    Task<Response<List<ScoreboardDtoResponse>>> GetTopScoresAsync();
}