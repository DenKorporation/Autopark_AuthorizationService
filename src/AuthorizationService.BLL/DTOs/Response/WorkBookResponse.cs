namespace AuthorizationService.BLL.DTOs.Response;

public record WorkBookResponse(
    Guid Id,
    string Number,
    DateOnly IssueDate,
    Guid UserId);
