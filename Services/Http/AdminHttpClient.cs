using CoreLibrary.DTOs.Category.Response;
using CoreLibrary.DTOs.Question.Requests;
using CoreLibrary.DTOs.Question.Response;
using CoreLibrary.Interface.Services.Client;
using CoreLibrary.Wrappers;
using System.Net.Http.Json;

namespace Services.Http;

public class AdminHttpClient : IAdminHttpClient
{
    private readonly HttpClient _httpClient;
    public AdminHttpClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Response<List<CategoryDtoResponse>>> GetAllCategoriesAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync("api/admin/categories");
            return await res.Content.ReadFromJsonAsync<Response<List<CategoryDtoResponse>>>()
                   ?? new Response<List<CategoryDtoResponse>>(null!) { Message = "Error al cargar categorías", Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<List<CategoryDtoResponse>>(null!) { Message = ex.Message, Succeeded = false };
        }
    }

    public async Task<Response<int>> CreateQuestionAsync(QuestionDtoRequest request)
    {
        try
        {
            var res = await _httpClient.PostAsJsonAsync("api/admin/questions", request);
            return await res.Content.ReadFromJsonAsync<Response<int>>()
                   ?? new Response<int>() { Message = "Error al crear pregunta", Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<int>() { Message = ex.Message, Succeeded = false };
        }
    }

    public async Task<Response<List<QuestionDtoResponse>>> GetAllQuestionsAsync()
    {
        try
        {
            var res = await _httpClient.GetAsync("api/admin/questions");
            return await res.Content.ReadFromJsonAsync<Response<List<QuestionDtoResponse>>>()
                   ?? new Response<List<QuestionDtoResponse>>(null!) { Message = "Error al cargar preguntas", Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<List<QuestionDtoResponse>>(null!) { Message = ex.Message, Succeeded = false };
        }
    }
}
