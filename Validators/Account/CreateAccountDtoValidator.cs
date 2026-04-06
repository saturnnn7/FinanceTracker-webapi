using FinanceTracker.DTOs.Account;
using FinanceTracker.Models.Enums;
using FluentValidation;

namespace FinanceTracker.Validators.Account;

public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    private static readonly string[] AllowedCurrencies = ["PLN", "USD", "EUR", "GBP", "RUB"];

    public CreateAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<AccountType>(t, true, out _))
            .WithMessage($"Type must be one of: {string.Join(", ", Enum.GetNames<AccountType>())}.");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3)
            .Must(c => AllowedCurrencies.Contains(c.ToUpper()))
            .WithMessage($"Currency must be one of: {string.Join(", ", AllowedCurrencies)}.");

        RuleFor(x => x.Color)
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("Color must be a valid HEX color (e.g. #FF5733).");
    }
}