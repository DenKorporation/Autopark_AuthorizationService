using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class UserNotFoundError(string code = "User.NotFound", string message = "User not found")
    : NotFoundError(code, message)
{
}
