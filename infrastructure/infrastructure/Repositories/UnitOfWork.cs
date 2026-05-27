using CoreLibrary.Interface.Repositories;
using Domain.Querys.Interface;
using infrastructure.Repositories.RepositoryAsync;
using infrastructure.Setting;
using System.Collections;

namespace infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ServiceContext _context;
    private readonly IQueryService _queries; 
    private Hashtable _repositories;

    public UnitOfWork(ServiceContext context, IQueryService queries)
    {
        _context = context;
        _queries = queries;
    }

    // Acceso a Dapper
    public IQueryService Queries => _queries;

    // Acceso a Repositorios EF
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories == null) _repositories = new Hashtable();
        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryInstance = new GenericRepository<TEntity>(_context);
            _repositories.Add(type, repositoryInstance);
        }
        return (IGenericRepository<TEntity>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();

    public async Task CommitnAsync()
    {
        await _context.SaveChangesAsync();

        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.CommitAsync();
            await currentTransaction.DisposeAsync();
        }
    }

    public async Task RollbackAsync()
    {
        var currentTransaction = _context.Database.CurrentTransaction;
        if (currentTransaction != null)
        {
            await currentTransaction.RollbackAsync();
            await currentTransaction.DisposeAsync();
        }
    }

    public void Dispose() => _context.Dispose();
}