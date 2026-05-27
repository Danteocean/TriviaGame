using Domain.Common; // Importamos la interfaz
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace infrastructure.Setting;

public class ServiceContext : DbContext
{
    public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entityTypes = Assembly.GetAssembly(typeof(IEntity))
            ?.GetTypes()
            .Where(t => typeof(IEntity).IsAssignableFrom(t)
                        && t.IsClass
                        && !t.IsAbstract);

        if (entityTypes != null)
        {
            foreach (var type in entityTypes)
            {
                modelBuilder.Entity(type);
            }
        }
    }
}