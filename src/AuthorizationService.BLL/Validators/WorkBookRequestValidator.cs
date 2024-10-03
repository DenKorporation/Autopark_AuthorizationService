using AuthorizationService.BLL.DTOs.Request;
using FluentValidation;

namespace AuthorizationService.BLL.Validators;

public class WorkBookRequestValidator : AbstractValidator<WorkBookRequest>
{
    public WorkBookRequestValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty()
            .WithMessage("Work book number was expected")
            .MaximumLength(20)
            .WithMessage("Length of work book number mustn't exceed 20");

        RuleFor(x => x.IssueDate)
            .NotEmpty()
            .WithMessage("Work book issue date was expected");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID was expected");
    }
}
