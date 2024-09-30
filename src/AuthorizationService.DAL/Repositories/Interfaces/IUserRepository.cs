using AuthorizationService.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.DAL.Repositories.Interfaces;

public interface IUserRepository
{
    public Task<IdentityResult> CreateAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<IQueryable<User>> GetAllAsync(CancellationToken cancellationToken = default);

    public Task<string> GetRoleAsync(
        User user,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> AssignRoleAsync(
        User user,
        string role,
        CancellationToken cancellationToken = default);

    public Task<bool> IsInRoleAsync(User user, string role, CancellationToken cancellationToken = default);

    public Task<IdentityResult> UpdateUserAsync(User user, CancellationToken cancellationToken = default);

    public Task<IdentityResult> ChangePasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> DeleteUserAsync(User user, CancellationToken cancellationToken = default);
}
