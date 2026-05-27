using CoreLibrary.DTOs.Question.Requests;
using CoreLibrary.Interface.Services;
using CoreLibrary.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace TriviaGameApi.Controllers;

[ApiController]
[Route("Question")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _service;

    public QuestionController(IQuestionService service) => _service = service;

    [HttpPost("Create")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] QuestionDtoRequest request) => Ok(await _service.CreateQuestionAsync(request));

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllQuestionsAsync());
}
