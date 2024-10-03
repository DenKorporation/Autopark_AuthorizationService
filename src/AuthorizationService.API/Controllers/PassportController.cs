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
[Route("api/v{apiVersion:apiVersion}/passports")]
public class PassportController(IPassportService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllPassportsAsync(
        [FromQuery] GetAllRequest getAllRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(getAllRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{passportId:guid}")]
    public async Task<IResult> GetPassportByIdAsync(Guid passportId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(passportId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost]
    public async Task<IResult> CreatePassportAsync(
        [FromBody] PassportRequest createPassportRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.CreateAsync(createPassportRequest, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut("{passportId:guid}")]
    public async Task<IResult> UpdatePassportAsync(
        Guid passportId,
        [FromBody] PassportRequest updatePassportRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.UpdateAsync(passportId, updatePassportRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{passportId:guid}")]
    public async Task<IResult> DeletePassportsAsync(Guid passportId, CancellationToken cancellationToken = default)
    {
        var result = await service.DeleteAsync(passportId, cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}
