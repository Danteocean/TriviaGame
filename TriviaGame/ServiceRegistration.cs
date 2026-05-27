namespace TriviaGame;
using CoreLibrary.Interface.Services.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Http;

public static class ServiceRegistration
{
    public static void AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
                         ?? "https://localhost:44358/";

        services.AddHttpClient<IPlayerHttpClient, PlayerHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddHttpClient<IGameSessionHttpClient, GameSessionHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        services.AddHttpClient<GameSessionHttpClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    }
}