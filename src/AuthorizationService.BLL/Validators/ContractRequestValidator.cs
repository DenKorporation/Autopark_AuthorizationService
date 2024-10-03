using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class ContractRequestValidator : AbstractValidator<ContractRequest>
{
    public ContractRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Contract number was expected")
            .MaximumLength(20)
            .WithMessage("Length of contract number mustn't exceed 20");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Contract start date was expected");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("Contract end date was expected");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID was expected");
    }
}
