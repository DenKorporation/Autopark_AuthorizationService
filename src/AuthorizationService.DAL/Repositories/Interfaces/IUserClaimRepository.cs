using System.Security.Claims;
using AuthorizationService.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.DAL.Repositories.Interfaces;

public interface IUserClaimRepository
{
    public Task<IdentityResult> AddClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> UpdateClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default);

    public Task<IdentityResult> DeleteClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default);
}
