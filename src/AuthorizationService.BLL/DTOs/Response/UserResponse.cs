namespace AuthorizationService.BLL.DTOs.Response;

public record UserResponse(
    Guid Id,
    string Email,
    string Role,
    Guid? WorkBookId,
    Guid? PassportId,
    List<Guid> ContractIds);
