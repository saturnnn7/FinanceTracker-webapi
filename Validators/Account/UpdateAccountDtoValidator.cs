using FinanceTracker.DTOs.Account;
using FluentValidation;

namespace FinanceTracker.Validators.Account;

public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Color)
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid HEX color (e.g. #FF5733).");
    }
}