using CoreLibrary.Wrappers;
using System.Net.Http.Json;

namespace Services.Common;

public abstract class BaseHttpClient
{
    protected readonly HttpClient _httpClient;

    protected BaseHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    protected async Task<Response<T>> SendGetAsync<T>(string url)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Response<T>>(url);
            return response ?? new Response<T>(default(T)) { Message = "Respuesta vacía del servidor", Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<T>(default(T)!) { Message = ex.Message, Succeeded = false };
        }
    }

    protected async Task<Response<TResponse>> SendPostAsync<TRequest, TResponse>(string url, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
                return new Response<TResponse>(default(TResponse)!) { Message = "Error en la petición: " + response.StatusCode, Succeeded = false };

            return await response.Content.ReadFromJsonAsync<Response<TResponse>>()
                   ?? new Response<TResponse>(default(TResponse)!) { Message = "Error al deserializar", Succeeded = false };
        }
        catch (Exception ex)
        {
            return new Response<TResponse>(default(TResponse)!) { Message = ex.Message, Succeeded = false };
        }
    }
}