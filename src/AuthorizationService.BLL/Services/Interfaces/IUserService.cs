using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using FluentResults;

namespace AuthorizationService.BLL.Services.Interfaces;

public interface IUserService
{
    public Task<Result<PagedList<UserResponse>>> GetAllAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<UserResponse>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    public Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Result<UserResponse>> CreateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<UserResponse>> UpdateAsync(
        Guid id,
        UserRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
