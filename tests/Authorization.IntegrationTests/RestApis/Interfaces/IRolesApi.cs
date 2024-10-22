using AuthorizationService.BLL.DTOs.Response;
using Refit;

namespace Authorization.IntegrationTests.RestApis.Interfaces;

public interface IRolesApi
{
    [Get("/roles")]
    Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllRolesAsync();
}
