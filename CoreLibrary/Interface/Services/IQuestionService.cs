using CoreLibrary.DTOs.Question.Requests;
using CoreLibrary.DTOs.Question.Response;
using CoreLibrary.Wrappers;

namespace CoreLibrary.Interface.Services;

public interface IQuestionService
{
    Task<Response<int>> CreateQuestionAsync(QuestionDtoRequest request);
    Task<Response<List<QuestionDtoResponse>>> GetAllQuestionsAsync();
}