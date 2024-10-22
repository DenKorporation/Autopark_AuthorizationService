namespace AuthorizationService.DAL.Models;

public class Contract
{
    public Guid Id { get; set; }

    public string Number { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
