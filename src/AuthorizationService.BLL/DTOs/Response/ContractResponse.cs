namespace AuthorizationService.BLL.DTOs.Response;

public record ContractResponse
{
    public ContractResponse()
    {
    }

    public ContractResponse(
        Guid id,
        string number,
        DateOnly startDate,
        DateOnly endDate,
        bool isValid,
        Guid userId)
    {
        Id = id;
        Number = number;
        StartDate = startDate;
        EndDate = endDate;
        IsValid = isValid;
        UserId = userId;
    }

    public Guid Id { get; init; }

    public string Number { get; init; } = null!;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public bool IsValid { get; init; }

    public Guid UserId { get; init; }

    public void Deconstruct(
        out Guid id,
        out string number,
        out DateOnly startDate,
        out DateOnly endDate,
        out bool isValid,
        out Guid userId)
    {
        id = Id;
        number = Number;
        startDate = StartDate;
        endDate = EndDate;
        isValid = IsValid;
        userId = UserId;
    }
}
