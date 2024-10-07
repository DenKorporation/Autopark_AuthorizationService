using Authorization.IntegrationTests.Responses;
using AuthorizationService.BLL.DTOs.Request;
using Refit;
using UserResponse = AuthorizationService.BLL.DTOs.Response.UserResponse;

namespace Authorization.IntegrationTests.RestApis.Interfaces;

public interface IUsersApi
{
    [Get("/users")]
    Task<ApiResponse<PagedList<UserResponse>>> GetAllUsersAsync(
        [Query] UserFilterRequest filterRequest,
        CancellationToken cancellationToken = default);

    [Get("/users/{userId}")]
    Task<ApiResponse<UserResponse>> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    [Post("/users")]
    Task<ApiResponse<UserResponse>> CreateUserAsync(
        [Body] UserRequest createUserRequest,
        CancellationToken cancellationToken = default);

    [Put("/users/{userId}")]
    Task<ApiResponse<UserResponse>> UpdateUserAsync(
        Guid userId,
        [Body] UserRequest updateUserRequest,
        CancellationToken cancellationToken = default);

    [Delete("/users/{userId}")]
    Task<ApiResponse<string>> DeleteUsersAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
