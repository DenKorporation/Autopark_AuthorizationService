namespace AuthorizationService.BLL.DTOs.Response;

public record UserResponse
{
    public UserResponse()
    {
    }

    public UserResponse(
        Guid id,
        string email,
        string role,
        Guid? workBookId,
        Guid? passportId,
        List<Guid> contractIds)
    {
        Id = id;
        Email = email;
        Role = role;
        WorkBookId = workBookId;
        PassportId = passportId;
        ContractIds = contractIds;
    }

    public Guid Id { get; init; }

    public string Email { get; init; } = null!;

    public string Role { get; init; } = null!;

    public Guid? WorkBookId { get; init; }

    public Guid? PassportId { get; init; }

    public List<Guid> ContractIds { get; init; } = null!;

    public void Deconstruct(
        out Guid id,
        out string email,
        out string role,
        out Guid? workBookId,
        out Guid? passportId,
        out List<Guid> contractIds)
    {
        id = Id;
        email = Email;
        role = Role;
        workBookId = WorkBookId;
        passportId = PassportId;
        contractIds = ContractIds;
    }
}
