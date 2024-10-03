namespace AuthorizationService.BLL.DTOs.Request;

public record ContractRequest(
    string Number,
    string StartDate,
    string EndDate,
    Guid UserId);
