using CoreLibrary.DTOs.Player.Requests;
using CoreLibrary.DTOs.Player.Response;
using CoreLibrary.Interface.Services;
using CoreLibrary.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace TriviaGameApi.Controllers;

[ApiController]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService) 
    {
        _playerService = playerService;
    } 

    [HttpPost("Register")]
    [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<int>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<int>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] PlayerDtoRequest request)
    {
        return Ok(await _playerService.RegisterPlayerAsync(request));
    }


    [HttpGet("GetById/{id}")]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _playerService.GetPlayerByIdAsync(id));
    }

    [HttpPost("EnterGame")]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<PlayerDtoResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EnterGame([FromBody] string alias)
    {
        return Ok(await _playerService.GetOrCreatePlayerByAliasAsync(alias));
    }
}