using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class UserRepository(AuthContext dbContext, UserManager<User> userManager)
    : IUserRepository
{
    public async Task<IdentityResult> CreateAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        return await userManager.CreateAsync(user, password);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await userManager.FindByIdAsync(id.ToString());
    }

    public Task<IQueryable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            userManager
                .Users
                .AsNoTracking());
    }

    public async Task<string> GetRoleAsync(User user, CancellationToken cancellationToken = default)
    {
        return (await userManager.GetRolesAsync(user)).First();
    }

    public async Task<IdentityResult> AssignRoleAsync(
        User user,
        string role,
        CancellationToken cancellationToken = default)
    {
        var result = await RemoveAllUserRoleAsync(user, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        return await userManager.AddToRoleAsync(user, role);
    }

    public async Task<bool> IsInRoleAsync(User user, string role, CancellationToken cancellationToken = default)
    {
        return await userManager.IsInRoleAsync(user, role);
    }

    public async Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return await userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangePasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var result = await userManager.RemovePasswordAsync(user);

        if (!result.Succeeded)
        {
            return result;
        }

        return await userManager.AddPasswordAsync(user, password);
    }

    public async Task<IdentityResult> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return await userManager.DeleteAsync(user);
    }

    public async Task LoadRelatedData(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Entry(user).Reference(u => u.Passport).LoadAsync(cancellationToken);
        await dbContext.Entry(user).Reference(u => u.WorkBook).LoadAsync(cancellationToken);
        await dbContext.Entry(user).Collection(u => u.Contracts).LoadAsync(cancellationToken);
        await dbContext.Entry(user).Collection(u => u.Roles).LoadAsync(cancellationToken);
    }

    private async Task<IdentityResult> RemoveAllUserRoleAsync(User user, CancellationToken cancellationToken = default)
    {
        return await userManager
            .RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
    }
}
