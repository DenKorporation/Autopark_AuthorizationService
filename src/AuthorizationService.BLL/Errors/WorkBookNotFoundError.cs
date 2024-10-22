using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class WorkBookNotFoundError(string code = "WorkBook.NotFound", string message = "WorkBook not found")
    : NotFoundError(code, message)
{
}
