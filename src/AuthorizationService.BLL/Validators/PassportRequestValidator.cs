using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class PassportRequestValidator : AbstractValidator<PassportRequest>
{
    public PassportRequestValidator()
    {
        RuleFor(x => x.Series)
            .NotEmpty()
            .WithMessage("Passport series was expected")
            .Length(2)
            .WithMessage("Length of passport series was expected to be 2");

        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Passport number was expected")
            .Length(7)
            .WithMessage("Length of passport number was expected to be 7");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty()
            .WithMessage("Passport Identification Number was expected")
            .Length(14)
            .WithMessage("Length of passport Identification Number was expected to be 14");

        RuleFor(x => x.Firstname)
            .NotEmpty()
            .WithMessage("Firstname was expected")
            .MaximumLength(100)
            .WithMessage("Length of firstname mustn't exceed 100");

        RuleFor(x => x.IdentificationNumber)
            .NotEmpty()
            .WithMessage("Lastname was expected")
            .MaximumLength(100)
            .WithMessage("Length of lastname mustn't exceed 100");

        RuleFor(x => x.Patronymic)
            .MaximumLength(100)
            .When(x => x.Patronymic is not null)
            .WithMessage("Length of patronymic mustn't exceed 100");

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .WithMessage("Passport issue date was expected")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Invalid date format. Expected format is 'yyyy-MM-dd'.");

        RuleFor(x => x.ExpiryDate)
            .NotEmpty()
            .WithMessage("Passport expiry date was expected")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Invalid date format. Expected format is 'yyyy-MM-dd'.");

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Birth date was expected")
            .Matches(@"^\d{4}-\d{2}-\d{2}$")
            .WithMessage("Invalid date format. Expected format is 'yyyy-MM-dd'.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID was expected");
    }
}
