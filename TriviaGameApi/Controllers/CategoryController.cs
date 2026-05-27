using CoreLibrary.Interface.Services;
using CoreLibrary.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace TriviaGameApi.Controllers;

[ApiController]
[Route("Category")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _categoryService.GetAllCategoriesAsync());
    }
}