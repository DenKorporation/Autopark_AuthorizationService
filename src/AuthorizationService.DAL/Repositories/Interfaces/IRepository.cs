namespace AuthorizationService.DAL.Repositories.Interfaces;

public interface IRepository<TEntity>
    where TEntity : class
{
    public Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default);
}