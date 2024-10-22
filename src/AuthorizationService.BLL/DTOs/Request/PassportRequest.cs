namespace AuthorizationService.BLL.DTOs.Request;

public record PassportRequest(
    string Series,
    string Number,
    string IdentificationNumber,
    string Firstname,
    string Lastname,
    string? Patronymic,
    string BirthDate,
    string IssueDate,
    string ExpiryDate,
    Guid UserId);
