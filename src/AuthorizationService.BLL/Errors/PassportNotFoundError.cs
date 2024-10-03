using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class PassportNotFoundError(string code = "Passport.NotFound", string message = "Passport not found")
    : NotFoundError(code, message)
{
}
