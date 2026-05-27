using Domain.Entities;
using Domain.Querys.Interface;

namespace CoreLibrary.Interface.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;


    IQueryService Queries { get; }

    Task BeginTransactionAsync();
    Task CommitnAsync();
    Task RollbackAsync();

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}