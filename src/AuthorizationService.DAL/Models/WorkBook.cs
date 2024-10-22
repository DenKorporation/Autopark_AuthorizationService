namespace AuthorizationService.DAL.Models;

public class WorkBook
{
    public Guid Id { get; set; }

    public string Number { get; set; } = null!;

    public DateOnly IssueDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
