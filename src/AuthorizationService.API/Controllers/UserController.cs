using Asp.Versioning;
using AuthorizationService.API.Extensions;
using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.Services.Interfaces;
using AuthorizationService.DAL.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationService.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Roles = $"{Roles.Administrator}, {Roles.FleetManager}, {Roles.HrManager}")]
[Route("api/v{apiVersion:apiVersion}/users")]
public class UserController(IUserService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllUsersAsync(
        [FromQuery] UserFilterRequest filterRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(filterRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{filterId:guid}")]
    public async Task<IResult> GetUserByIdAsync(Guid filterId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(filterId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{filterEmail}")]
    public async Task<IResult> GetUserByIdAsync(string filterEmail, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByEmailAsync(filterEmail, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost]
    public async Task<IResult> CreateUserAsync(
        [FromBody] UserRequest createUserRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.CreateAsync(createUserRequest, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut("{userId:guid}")]
    public async Task<IResult> UpdateUserAsync(
        Guid userId,
        [FromBody] UserRequest updateUserRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.UpdateAsync(userId, updateUserRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await service.DeleteAsync(userId, cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}
