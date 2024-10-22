using Microsoft.AspNetCore.Identity;

namespace AuthorizationService.DAL.Models;

public class User : IdentityUser<Guid>
{
    public WorkBook? WorkBook { get; set; }

    public Passport? Passport { get; set; }

    public ICollection<Contract> Contracts { get; set; } = null!;

    public ICollection<IdentityRole<Guid>> Roles { get; set; } = null!;
}
