using Authorization.IntegrationTests.Responses;
using AuthorizationService.BLL.DTOs.Request;
using Refit;
using ContractResponse = AuthorizationService.BLL.DTOs.Response.ContractResponse;

namespace Authorization.IntegrationTests.RestApis.Interfaces;

public interface IContractsApi
{
    [Get("/contracts")]
    Task<ApiResponse<PagedList<ContractResponse>>> GetAllContractsAsync(
        [Query] ContractFilterRequest filterRequest,
        CancellationToken cancellationToken = default);

    [Get("/contracts/{contractId}")]
    Task<ApiResponse<ContractResponse>> GetContractByIdAsync(
        Guid contractId,
        CancellationToken cancellationToken = default);

    [Post("/contracts")]
    Task<ApiResponse<ContractResponse>> CreateContractAsync(
        [Body] ContractRequest createContractRequest,
        CancellationToken cancellationToken = default);

    [Put("/contracts/{contractId}")]
    Task<ApiResponse<ContractResponse>> UpdateContractAsync(
        Guid contractId,
        [Body] ContractRequest updateContractRequest,
        CancellationToken cancellationToken = default);

    [Delete("/contracts/{contractId}")]
    Task<ApiResponse<string>> DeleteContractsAsync(
        Guid contractId,
        CancellationToken cancellationToken = default);
}
