namespace AuthorizationService.BLL.DTOs.Request;

public record ContractRequest(
    string Number,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid UserId);
