namespace AuthorizationService.BLL.DTOs.Request;

public record UserFilterRequest(
    int PageNumber,
    int PageSize,
    string? Role,
    DateOnly? BirthdateFrom,
    DateOnly? BirthdateTo);
