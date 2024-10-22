namespace AuthorizationService.BLL.DTOs.Response;

public record PassportResponse(
    Guid Id,
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
