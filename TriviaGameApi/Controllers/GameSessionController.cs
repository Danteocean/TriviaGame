using CoreLibrary.DTOs.GameSession.Requests;
using CoreLibrary.Interface.Services;
using CoreLibrary.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace TriviaGameApi.Controllers;

[ApiController]
[Route("GameSession")]
public class GameSessionController : ControllerBase
{
    private readonly IGameSessionService _gameSessionService;

    public GameSessionController(IGameSessionService gameSessionService)
    {
        _gameSessionService = gameSessionService;
    }

    [HttpGet("GetById/{sessionId}")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid sessionId)
    {
        return Ok(await _gameSessionService.GetByIdAsync(sessionId));
    }

    [HttpPost("StartGame")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> StartGame([FromBody] int playerId)
    {
        return Ok(await _gameSessionService.StartGameAsync(playerId));
    }

    [HttpGet("GetNextQuestion/{sessionId}")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetNextQuestion(Guid sessionId)
    {
        return Ok(await _gameSessionService.GetNextQuestionAsync(sessionId));
    }

    [HttpPost("SubmitAnswer")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitAnswer([FromBody] AnswerDtoRequest request)
    {
        return Ok(await _gameSessionService.SubmitAnswerAsync(request));
    }

    [HttpPost("Withdraw")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Withdraw([FromBody] Guid sessionId)
    {
        return Ok(await _gameSessionService.WithdrawAsync(sessionId));
    }
}