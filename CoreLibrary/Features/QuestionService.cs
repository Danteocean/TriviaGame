using AutoMapper;
using CoreLibrary.DTOs.Question.Response;
using CoreLibrary.DTOs.Question.Requests;
using CoreLibrary.Interface.Services;
using Domain.Entities;
using CoreLibrary.Wrappers;
using CoreLibrary.Interface.Repositories;
using Domain.Querys;

namespace CoreLibrary.Features;

public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Response<int>> CreateQuestionAsync(QuestionDtoRequest request)
    {
        try
        {
            var question = _mapper.Map<Question>(request);
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Question>().AddAsync(question);
            await _unitOfWork.CommitnAsync();
            return new Response<int>(question.Id) { State = "Ok", Succeeded = true };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new Response<int>(0) { State = "Error", Message = ex.Message, Succeeded = false };
        }
    }

    public async Task<Response<List<QuestionDtoResponse>>> GetAllQuestionsAsync()
    {
        var questionDictionary = new Dictionary<int, QuestionDtoResponse>();

        var (data, message) = await _unitOfWork.Queries.QueryMultiMapAsync<QuestionDtoResponse, OptionDtoResponse, QuestionDtoResponse>(
            SqlQueries.GetAllQuestions,
            (question, option) =>
            {
                if (!questionDictionary.TryGetValue(question.QuestionId, out var currentQuestion))
                {
                    currentQuestion = question;
                    currentQuestion.Options = new List<OptionDtoResponse>();
                    questionDictionary.Add(currentQuestion.QuestionId, currentQuestion);
                }

                if (option != null) currentQuestion.Options.Add(option);
                return currentQuestion;
            },
            splitOn: "OptionId"
        );

        if (data != null)
        {
            return new Response<List<QuestionDtoResponse>>(questionDictionary.Values.ToList())
            { State = "Ok", Message = "Success", Succeeded = true };
        }

        return new Response<List<QuestionDtoResponse>>(null!) { State = "NoData", Message = message, Succeeded = false };
    }
}