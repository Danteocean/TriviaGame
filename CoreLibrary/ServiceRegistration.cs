using CoreLibrary.Features;
using CoreLibrary.Interface.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Microservice.core;

public static class ServiceRegistration
{
    public static void AddCoreLayer(this IServiceCollection services)
    {
        services.AddTransient<IGameSessionService, GameSessionService>();
        services.AddTransient<IPlayerService, PlayerService>();
        services.AddTransient<IQuestionService, QuestionService>();
        services.AddTransient<ICategoryService, CategoryService>();
        services.AddTransient<IScoreboardService, ScoreboardService>();
        services.AddAutoMapper((cfg) => { }, AppDomain.CurrentDomain.GetAssemblies());
    }
}