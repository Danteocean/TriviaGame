using CoreLibrary.DTOs.Category.Response;
using CoreLibrary.DTOs.Question.Requests;
using CoreLibrary.DTOs.Question.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface IAdminService
{
    Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync();
    Task<Response<int>> CreateQuestionAsync(QuestionDtoRequest request);
    Task<Response<List<QuestionDtoResponse>>> GetAllQuestionsAsync();
}
