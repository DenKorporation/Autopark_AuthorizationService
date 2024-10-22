using System.Security.Claims;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class UserClaimRepository(UserManager<User> userManager)
    : IUserClaimRepository
{
    public async Task<IdentityResult> AddClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default)
    {
        return await userManager.AddClaimsAsync(user, claims);
    }

    public async Task<IdentityResult> UpdateClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default)
    {
        var oldClaims = await userManager.GetClaimsAsync(user);

        foreach (var claim in claims)
        {
            var oldClaim = oldClaims.FirstOrDefault(c => c.Type == claim.Type);

            IdentityResult result;

            if (oldClaim is null)
            {
                result = await userManager.AddClaimAsync(user, claim);
            }
            else
            {
                result = await userManager.ReplaceClaimAsync(user, oldClaim, claim);
            }

            if (result is { Succeeded: false })
            {
                throw new Exception(result.Errors.First().Description);
            }
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteClaimsAsync(
        User user,
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default)
    {
       return await userManager.RemoveClaimsAsync(user, claims);
    }
}
