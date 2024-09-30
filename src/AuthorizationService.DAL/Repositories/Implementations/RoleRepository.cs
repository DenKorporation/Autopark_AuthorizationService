using AuthorizationService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class RoleRepository(RoleManager<IdentityRole<Guid>> roleManager)
    : IRoleRepository
{
    public async Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await roleManager
            .RoleExistsAsync(roleName);
    }

    public Task<IQueryable<IdentityRole<Guid>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            roleManager
                .Roles
                .AsNoTracking());
    }
}
