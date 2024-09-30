using System.Security.Claims;
using AuthorizationService.DAL.Models;
using IdentityModel;

namespace AuthorizationService.DAL.Extensions;

public static class ClaimsExtensions
{
    public static List<Claim> ToClaims(this User user)
    {
        return
        [
            new Claim(JwtClaimTypes.Email, user.Email!),
        ];
    }

    public static List<Claim> ToClaims(this Passport passport)
    {
        return
        [
            new Claim(JwtClaimTypes.GivenName, passport.Firstname),
            new Claim(JwtClaimTypes.FamilyName, passport.Lastname),
            new Claim(JwtClaimTypes.BirthDate, passport.BirthDate.ToString("O")),
        ];
    }
}
