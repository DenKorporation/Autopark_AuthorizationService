namespace AuthorizationService.DAL.Models;

public class Passport
{
    public Guid Id { get; set; }

    public string Series { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string IdentificationNumber { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public DateOnly BirthDate { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
