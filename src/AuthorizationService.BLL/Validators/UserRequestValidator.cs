using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email was expected")
            .EmailAddress()
            .WithMessage("Incorrect email format")
            .MaximumLength(256)
            .WithMessage("Length of email mustn't exceed 256");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role was expected")
            .MaximumLength(256)
            .WithMessage("Length of role mustn't exceed 256");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password was expected")
            .MinimumLength(6)
            .WithMessage("Password must contain at least 6 character")
            .Matches("[A-Z]")
            .WithMessage("Password must contain uppercase character")
            .Matches("[a-z]")
            .WithMessage("Password must contain lowercase character")
            .Matches("[0-9]")
            .WithMessage("Password must contain digit")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Password must contain non-alphanumeric character")
            .When(x => x.Password != null);
    }
}
