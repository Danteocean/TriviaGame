using AutoMapper;
using CoreLibrary.DTOs.Player.Response;
using CoreLibrary.DTOs.Player.Requests;
using CoreLibrary.Interface.Services;
using Domain.Entities;
using CoreLibrary.Wrappers;
using CoreLibrary.Interface.Repositories;
using Domain.Querys;

namespace CoreLibrary.Features;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlayerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Response<int>> RegisterPlayerAsync(PlayerDtoRequest request)
    {
        try
        {
            var player = _mapper.Map<Player>(request);
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Player>().AddAsync(player);
            await _unitOfWork.CommitnAsync();
            return new Response<int>(player.id) { State = "Ok", Succeeded = true };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<int>(0) { State = "Error", Message = ex.Message, Succeeded = false };
        }
    }

    public async Task<Response<PlayerDtoResponse>> GetPlayerByIdAsync(int id)
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetPlayerById, new { Id = id });

            if (data != null && data.Any())
            {
                var result = _mapper.Map<PlayerDtoResponse>(data.FirstOrDefault());
                return new Response<PlayerDtoResponse>(result) { State = "Ok", Message = message, Succeeded = true };
            }
            return new Response<PlayerDtoResponse>(null) { State = "NoData", Message = message, Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<PlayerDtoResponse>(null) { State = "NoData", Message = ex.Message, Succeeded = false };
        }
      
    }

    public async Task<Response<PlayerDtoResponse>> GetOrCreatePlayerByAliasAsync(string alias)
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetPlayerByAlias, new { Alias = alias });

            if (data != null && data.Any())
            {
                var result = _mapper.Map<PlayerDtoResponse>(data.FirstOrDefault());
                return new Response<PlayerDtoResponse>(result) { State = "Ok", Message = "Jugador recuperado", Succeeded = true };
            }

            // 2. Si no existe, crear uno nuevo
            var player = new Player { alias = alias };
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Player>().AddAsync(player);
            await _unitOfWork.CommitnAsync();

            return new Response<PlayerDtoResponse>(_mapper.Map<PlayerDtoResponse>(player))
            {
                State = "Ok",
                Message = "Jugador creado",
                Succeeded = true
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<PlayerDtoResponse>(null!) { State = "Error", Message = ex.Message, Succeeded = false };
        }
    }
}