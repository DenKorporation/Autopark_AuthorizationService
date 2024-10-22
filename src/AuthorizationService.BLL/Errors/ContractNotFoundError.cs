using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class ContractNotFoundError(string code = "Contract.NotFound", string message = "Contract not found")
    : NotFoundError(code, message)
{
}
