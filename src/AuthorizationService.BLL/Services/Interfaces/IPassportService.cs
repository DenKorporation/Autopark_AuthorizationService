using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using FluentResults;

namespace AuthorizationService.BLL.Services.Interfaces;

public interface IPassportService
{
    public Task<Result<PagedList<PassportResponse>>> GetAllAsync(
        GetAllRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<PassportResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Result<PassportResponse>> CreateAsync(
        PassportRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<PassportResponse>> UpdateAsync(
        Guid id,
        PassportRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
