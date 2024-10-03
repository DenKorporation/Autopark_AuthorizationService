namespace AuthorizationService.BLL.DTOs.Response;

public record ContractResponse(
    Guid Id,
    string Number,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsValid,
    Guid UserId);
