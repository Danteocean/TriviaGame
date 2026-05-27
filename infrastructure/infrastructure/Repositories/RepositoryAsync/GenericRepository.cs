using CoreLibrary.Interface.Repositories;
using infrastructure.Extensions;
using infrastructure.Setting;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repositories.RepositoryAsync;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly ServiceContext _dbContext;
    protected readonly DbSet<TEntity> _entities;

    public GenericRepository(ServiceContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _entities = _dbContext.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        ValidateNullEntity<TEntity>.IsNullEntity(entity);

        await _entities.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        ValidateNullEntity<TEntity>.IsNullEntity(entity);

        _entities.Update(entity);
        return entity;
    }
}