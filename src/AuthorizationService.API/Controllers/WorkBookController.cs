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
[Route("api/v{apiVersion:apiVersion}/workBooks")]
public class WorkBookController(IWorkBookService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllWorkBooksAsync(
        [FromQuery] GetAllRequest getAllRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(getAllRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{workBookId:guid}")]
    public async Task<IResult> GetWorkBookByIdAsync(Guid workBookId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(workBookId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost]
    public async Task<IResult> CreateWorkBookAsync(
        [FromBody] WorkBookRequest createWorkBookRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.CreateAsync(createWorkBookRequest, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut("{workBookId:guid}")]
    public async Task<IResult> UpdateWorkBookAsync(
        Guid workBookId,
        [FromBody] WorkBookRequest updateWorkBookRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.UpdateAsync(workBookId, updateWorkBookRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{workBookId:guid}")]
    public async Task<IResult> DeleteWorkBooksAsync(Guid workBookId, CancellationToken cancellationToken = default)
    {
        var result = await service.DeleteAsync(workBookId, cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}
