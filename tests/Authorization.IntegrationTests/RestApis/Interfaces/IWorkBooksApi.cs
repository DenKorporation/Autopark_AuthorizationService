using Authorization.IntegrationTests.Responses;
using AuthorizationService.BLL.DTOs.Request;
using Refit;
using WorkBookResponse = AuthorizationService.BLL.DTOs.Response.WorkBookResponse;

namespace Authorization.IntegrationTests.RestApis.Interfaces;

public interface IWorkBooksApi
{
    [Get("/workBooks")]
    Task<ApiResponse<PagedList<WorkBookResponse>>> GetAllWorkBooksAsync(
        [Query] GetAllRequest filterRequest,
        CancellationToken cancellationToken = default);

    [Get("/workBooks/{workBookId}")]
    Task<ApiResponse<WorkBookResponse>> GetWorkBookByIdAsync(
        Guid workBookId,
        CancellationToken cancellationToken = default);

    [Post("/workBooks")]
    Task<ApiResponse<WorkBookResponse>> CreateWorkBookAsync(
        [Body] WorkBookRequest createWorkBookRequest,
        CancellationToken cancellationToken = default);

    [Put("/workBooks/{workBookId}")]
    Task<ApiResponse<WorkBookResponse>> UpdateWorkBookAsync(
        Guid workBookId,
        [Body] WorkBookRequest updateWorkBookRequest,
        CancellationToken cancellationToken = default);

    [Delete("/workBooks/{workBookId}")]
    Task<ApiResponse<string>> DeleteWorkBooksAsync(
        Guid workBookId,
        CancellationToken cancellationToken = default);
}
