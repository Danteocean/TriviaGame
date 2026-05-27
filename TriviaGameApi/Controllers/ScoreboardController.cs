using CoreLibrary.Interface.Services;
using CoreLibrary.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace TriviaGameApi.Controllers;

[ApiController]
[Route("Scoreboard")]
public class ScoreboardController : ControllerBase
{
    private readonly IScoreboardService _scoreboardService;

    public ScoreboardController(IScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    [HttpGet("GetTop")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTop()
    {
        return Ok(await _scoreboardService.GetTopScoresAsync());
    }
}