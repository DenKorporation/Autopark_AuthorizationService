using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using FluentResults;

namespace AuthorizationService.BLL.Services.Interfaces;

public interface IContractService
{
    public Task<Result<PagedList<ContractResponse>>> GetAllAsync(
        ContractFilterRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<ContractResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Result<ContractResponse>> CreateAsync(
        ContractRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<ContractResponse>> UpdateAsync(
        Guid id,
        ContractRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
