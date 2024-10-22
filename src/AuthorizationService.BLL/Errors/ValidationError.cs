using AuthorizationService.BLL.Errors.Base;

namespace AuthorizationService.BLL.Errors;

public class ValidationError(
    Dictionary<string, string[]> errors,
    string code = "Validation",
    string message = "Validation errors occurred")
    : BadRequestError(code, message)
{
    public Dictionary<string, string[]> ValidationErrors { get; } = errors;
}
