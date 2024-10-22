using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class Repository<TEntity>(AuthContext dbContext)
    : IRepository<TEntity>
    where TEntity : class
{
    public Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            dbContext
                .Set<TEntity>()
                .AsNoTracking()
                .OrderBy(e => EF.Property<Guid>(e, "Id"))
                .AsQueryable());
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbContext
            .Set<TEntity>()
            .AddAsync(entity, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext
            .Set<TEntity>()
            .Update(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext
            .Set<TEntity>()
            .Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext
            .Set<TEntity>()
            .AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    }
}
