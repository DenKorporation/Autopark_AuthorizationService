namespace AuthorizationService.BLL.DTOs.Request;

public record WorkBookRequest(
    string Number,
    DateOnly IssueDate,
    Guid UserId);
