using CoreLibrary.DTOs.Category.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface ICategoryService
{
    Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync();
}