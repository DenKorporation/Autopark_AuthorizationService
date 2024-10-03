namespace AuthorizationService.BLL.DTOs.Request;

public record ContractFilterRequest(
    int PageNumber,
    int PageSize,
    string? Number,
    DateOnly? StartDate,
    DateOnly? EndDate,
    bool? IsValid,
    Guid? UserId);