namespace AuthorizationService.BLL.Errors.Base;

public class InternalServerError(string code, string message)
    : BaseError(code, message)
{
}
