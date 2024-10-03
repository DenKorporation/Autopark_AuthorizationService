using Asp.Versioning;
using AuthorizationService.API.Extensions;
using AuthorizationService.BLL.Services.Interfaces;
using AuthorizationService.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Roles = $"{Roles.Administrator}, {Roles.FleetManager}, {Roles.HrManager}")]
[Route("api/v{apiVersion:apiVersion}/roles")]
public class RoleController(IRoleService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllRolesAsync(cancellationToken);

        return result.ToAspResult(Results.Ok);
    }
}
