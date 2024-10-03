using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class RoleNotFoundError(string code = "Role.NotFound", string message = "Role not found")
    : NotFoundError(code, message)
{
}
