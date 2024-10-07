using Authorization.IntegrationTests.Responses;
using AuthorizationService.BLL.DTOs.Request;
using Refit;
using PassportResponse = AuthorizationService.BLL.DTOs.Response.PassportResponse;

namespace Authorization.IntegrationTests.RestApis.Interfaces;

public interface IPassportsApi
{
    [Get("/passports")]
    Task<ApiResponse<PagedList<PassportResponse>>> GetAllPassportsAsync(
        [Query] GetAllRequest filterRequest,
        CancellationToken cancellationToken = default);

    [Get("/passports/{passportId}")]
    Task<ApiResponse<PassportResponse>> GetPassportByIdAsync(
        Guid passportId,
        CancellationToken cancellationToken = default);

    [Post("/passports")]
    Task<ApiResponse<PassportResponse>> CreatePassportAsync(
        [Body] PassportRequest createPassportRequest,
        CancellationToken cancellationToken = default);

    [Put("/passports/{passportId}")]
    Task<ApiResponse<PassportResponse>> UpdatePassportAsync(
        Guid passportId,
        [Body] PassportRequest updatePassportRequest,
        CancellationToken cancellationToken = default);

    [Delete("/passports/{passportId}")]
    Task<ApiResponse<string>> DeletePassportsAsync(
        Guid passportId,
        CancellationToken cancellationToken = default);
}
