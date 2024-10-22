namespace AuthorizationService.BLL.DTOs.Request;

public record UserFilterRequest(
    int PageNumber,
    int PageSize,
    string? Role,
    string? BirthdateFrom,
    string? BirthdateTo);
