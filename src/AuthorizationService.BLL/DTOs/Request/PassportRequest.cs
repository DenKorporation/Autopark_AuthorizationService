namespace AuthorizationService.BLL.DTOs.Request;

public record PassportRequest(
    string Series,
    string Number,
    string IdentificationNumber,
    string Firstname,
    string Lastname,
    string? Patronymic,
    DateOnly BirthDate,
    DateOnly IssueDate,
    DateOnly ExpiryDate,
    Guid UserId);
