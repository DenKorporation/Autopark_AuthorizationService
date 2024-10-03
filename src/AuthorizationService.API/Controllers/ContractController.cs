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
[Route("api/v{apiVersion:apiVersion}/contracts")]
public class ContractController(IContractService service)
    : ControllerBase
{
    [HttpGet("")]
    public async Task<IResult> GetAllContractsAsync(
        [FromQuery] ContractFilterRequest filterRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(filterRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpGet("{contractId:guid}")]
    public async Task<IResult> GetContractByIdAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var result = await service.GetByIdAsync(contractId, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpPost]
    public async Task<IResult> CreateContractAsync(
        [FromBody] ContractRequest createContractRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.CreateAsync(createContractRequest, cancellationToken);

        return result.ToAspResult(value => Results.Created(string.Empty, value));
    }

    [HttpPut("{contractId:guid}")]
    public async Task<IResult> UpdateContractAsync(
        Guid contractId,
        [FromBody] ContractRequest updateContractRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await service.UpdateAsync(contractId, updateContractRequest, cancellationToken);

        return result.ToAspResult(Results.Ok);
    }

    [HttpDelete("{contractId:guid}")]
    public async Task<IResult> DeleteContractsAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var result = await service.DeleteAsync(contractId, cancellationToken);

        return result.ToAspResult(Results.NoContent);
    }
}
