using AutoMapper;
using CoreLibrary.DTOs.Scoreboard.Response;
using CoreLibrary.Interface.Services;
using CoreLibrary.Interface.Repositories;
using CoreLibrary.Wrappers;
using Domain.Querys;

namespace CoreLibrary.Features;

public class ScoreboardService : IScoreboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ScoreboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Response<List<ScoreboardDtoResponse>>> GetTopScoresAsync()
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetTopScores, null);

            if (data != null && data.Any())
            {
                var result = _mapper.Map<List<ScoreboardDtoResponse>>(data);
                return new Response<List<ScoreboardDtoResponse>>(result)
                { State = "Ok", Message = message, Succeeded = true };
            }

            return new Response<List<ScoreboardDtoResponse>>(null)
            { State = "NoData", Message = message, Succeeded = true };
        }
        catch (Exception ex)
        {
            return new Response<List<ScoreboardDtoResponse>>(null)
            { State = "Error", Message = ex.Message, Succeeded = false };
        }
    }
}