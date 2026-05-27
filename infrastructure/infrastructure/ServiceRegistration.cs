using CoreLibrary.Interface.Repositories;
using infrastructure.Repositories;
using infrastructure.Setting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services,
     IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Missing DefaultConnection");


        services.AddDbContext<ServiceContext>(options =>
            options.UseSqlServer(conn));

        return services;
    }


    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
