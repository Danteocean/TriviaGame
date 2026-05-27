using AutoMapper;
using CoreLibrary.DTOs.GameSession.Response;
using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.Interface.Services;
using Domain.Entities;
using CoreLibrary.Wrappers;
using CoreLibrary.Interface.Repositories;
using Domain.Querys;

namespace CoreLibrary.Features;

public class GameSessionService : IGameSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GameSessionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Response<GameResultDtoResponse>> GetByIdAsync(Guid sessionId)
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetGameSessionById, new { Id = sessionId });

            if (data != null && data.Any())
            {
                var result = _mapper.Map<GameResultDtoResponse>(data.FirstOrDefault());
                return new Response<GameResultDtoResponse>(result) { State = "Ok", Message = message, Succeeded = true };
            }
            return new Response<GameResultDtoResponse>(null) { State = "NoData", Message = message, Succeeded = true };
        }
        catch (Exception ex)
        {
            return new Response<GameResultDtoResponse>(null)
            { State = "NoData", Message = ex.Message, Succeeded = false };
        }
       
    }

    public async Task<Response<QuestionDtoResponse>> GetNextQuestionAsync(Guid sessionId)
    {
        try
        {
            var (data, message) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetGameSessionById, new { Id = sessionId });
            var session = data?.FirstOrDefault();

            if (session == null) return new Response<QuestionDtoResponse>(null) { Message = "Sesión no encontrada" };

            var question = await _unitOfWork.Queries.QueryFirstOrDefaultAsync<QuestionDtoResponse>(SqlQueries.GetRandomQuestionByLevel, new { Level = session.CurrentRound });

            if (question.Data != null)
            {
                var options = await _unitOfWork.Queries.QueryAsync<OptionDtoResponse>(SqlQueries.GetOptionsByQuestion, new { QuestionId = question.Data.QuestionId });

                question.Data.Options = _mapper.Map<List<OptionDtoResponse>>(options.Data);
            }

            return new Response<QuestionDtoResponse>(_mapper.Map<QuestionDtoResponse>(question)) { Succeeded = true };
        }
        catch (Exception ex)
        {
            return new Response<QuestionDtoResponse>(null)
            { State = "NoData", Message = ex.Message, Succeeded = false };
        }

    }

    public async Task<Response<GameResultDtoResponse>> SubmitAnswerAsync(AnswerDtoRequest request)
    {
        // 1. Obtener la sesión actual usando Dapper
        var (sessionData, _) = await _unitOfWork.Queries.QueryAsync<dynamic>(SqlQueries.GetGameSessionById, new { Id = request.SessionId });
        var sessionEntity = sessionData?.FirstOrDefault();

        if (sessionEntity == null)
        {
            return new Response<GameResultDtoResponse>(null) { Message = "Sesión no encontrada", Succeeded = false };
        }
           

        // 2. Validar respuesta
        var isCorrect = await _unitOfWork.Queries.QueryFirstOrDefaultAsync<bool>(SqlQueries.CheckIfAnswerIsCorrect, new { OptionId = request.OptionId });

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (!isCorrect.Data)
            {
                // El jugador pierde
                sessionEntity.Status = "Lost";
                await _unitOfWork.Repository<GameSession>().UpdateAsync(sessionEntity);
                await _unitOfWork.CommitnAsync();

                return new Response<GameResultDtoResponse>(new GameResultDtoResponse
                {
                    IsCorrect = false,
                    IsGameOver = true
                })
                { Succeeded = true };
            }

            // 3. Lógica de avance: Incrementar ronda y premio
            sessionEntity.CurrentRound++;
            sessionEntity.AccumulatedPrize += 1000;

            // Si supera la ronda 5, gana el juego
            if (sessionEntity.CurrentRound > 5)
            {
                sessionEntity.Status = "Won";
            }

            await _unitOfWork.Repository<GameSession>().UpdateAsync(sessionEntity);
            await _unitOfWork.CommitnAsync();

            return new Response<GameResultDtoResponse>(new GameResultDtoResponse
            {
                IsCorrect = true,
                IsGameOver = sessionEntity.Status == "Won" || sessionEntity.Status == "Lost",
                CurrentRound = sessionEntity.CurrentRound,
                AccumulatedPrize = sessionEntity.AccumulatedPrize
            })
            { Succeeded = true };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<GameResultDtoResponse>(null) { Message = ex.Message, Succeeded = false };
        }
    }

    public async Task<Response<Guid>> StartGameAsync(int playerId)
    {
        try
        {
            var session = new GameSession { PlayerId = playerId, CurrentRound = 1, Status = "In-Progress" };
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<GameSession>().AddAsync(session);
            await _unitOfWork.CommitnAsync();
            return new Response<Guid>(session.Id) { Succeeded = true };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<Guid>(playerId) { Message = ex.Message, Succeeded = false };
        }
        
    }

    public async Task<Response<bool>> WithdrawAsync(Guid sessionId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<GameSession>().UpdateAsync(new GameSession { Id = sessionId, Status = "Withdrawn" });
            await _unitOfWork.CommitnAsync();
            return new Response<bool>(true) { Succeeded = true };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<bool>(false) { Message = ex.Message, Succeeded = false };
        }
      
    }
}