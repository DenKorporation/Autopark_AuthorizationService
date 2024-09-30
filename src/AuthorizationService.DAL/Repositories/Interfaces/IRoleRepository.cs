using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.DAL.Repositories.Interfaces;

public interface IRoleRepository
{
    public Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default);
    public Task<IQueryable<IdentityRole<Guid>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
}
