using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class ContractFilterRequestValidator : AbstractValidator<ContractFilterRequest>
{
    public ContractFilterRequestValidator()
    {
        RuleFor(x => x.Number)
            .MaximumLength(20)
            .When(x => x.Number is not null)
            .WithMessage("Length of contract number mustn't exceed 20");

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
    }
}
