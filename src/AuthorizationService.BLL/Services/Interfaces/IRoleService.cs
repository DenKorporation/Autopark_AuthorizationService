using AuthorizationService.BLL.DTOs.Response;
using FluentResults;

namespace AuthorizationService.BLL.Services.Interfaces;

public interface IRoleService
{
    Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
}
