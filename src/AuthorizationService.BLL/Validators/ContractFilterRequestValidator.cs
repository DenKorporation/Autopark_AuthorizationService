using System.Data;
using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class ContractFilterRequestValidator : AbstractValidator<ContractFilterRequest>
{
    public ContractFilterRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .NotNull()
            .WithMessage("The page number was expected")
            .GreaterThan(0)
            .WithMessage("The page number must be more than 0");

        RuleFor(x => x.PageSize)
            .NotNull()
            .WithMessage("The page size was expected")
            .GreaterThan(0)
            .WithMessage("The page size must be more than 0");

        RuleFor(x => x.Number)
            .MaximumLength(20)
            .WithMessage("Length of contract number mustn't exceed 20")
            .When(x => x.Number is not null);

        RuleFor(x => x.StartDate)
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Invalid date format. Expected format is 'yyyy-MM-dd'.")
            .When(x => x.StartDate is not null);

        RuleFor(x => x.EndDate)
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Invalid date format. Expected format is 'yyyy-MM-dd'.")
            .When(x => x.EndDate is not null);
    }
}
