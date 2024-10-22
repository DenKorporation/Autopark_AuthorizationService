using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using FluentResults;

namespace AuthorizationService.BLL.Services.Interfaces;

public interface IWorkBookService
{
    public Task<Result<PagedList<WorkBookResponse>>> GetAllAsync(
        GetAllRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<WorkBookResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public Task<Result<WorkBookResponse>> CreateAsync(
        WorkBookRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result<WorkBookResponse>> UpdateAsync(
        Guid id,
        WorkBookRequest request,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
