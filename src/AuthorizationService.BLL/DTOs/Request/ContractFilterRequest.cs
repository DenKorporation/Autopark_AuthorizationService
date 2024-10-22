namespace AuthorizationService.BLL.DTOs.Request;

public record ContractFilterRequest(
    int PageNumber,
    int PageSize,
    string? Number,
    string? StartDate,
    string? EndDate,
    bool? IsValid,
    Guid? UserId);