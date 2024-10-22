namespace AuthorizationService.BLL.Errors.Base;

public class NotFoundError(string code, string message)
    : BaseError(code, message)
{
}
