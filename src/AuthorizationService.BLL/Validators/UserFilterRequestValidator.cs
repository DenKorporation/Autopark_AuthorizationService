using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class UserFilterRequestValidator : AbstractValidator<UserFilterRequest>
{
    public UserFilterRequestValidator()
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

        RuleFor(x => x.Role)
            .MaximumLength(256)
            .When(x => x.Role is not null)
            .WithMessage("Length of role mustn't exceed 256");
    }
}
