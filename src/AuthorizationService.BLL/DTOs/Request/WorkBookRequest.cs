namespace AuthorizationService.BLL.DTOs.Request;

public record WorkBookRequest(
    string Number,
    string IssueDate,
    Guid UserId);
