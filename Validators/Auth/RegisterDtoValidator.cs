using FinanceTracker.DTOs.Auth;
using FluentValidation;

namespace FinanceTracker.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private static readonly string[] AllowedCurrencies = ["PLN", "USD", "EUR", "GBP", "RUB"];

    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, digits and underscores.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .MaximumLength(100)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.BaseCurrency)
            .NotEmpty()
            .MaximumLength(3)
            .Must(c => AllowedCurrencies.Contains(c.ToUpper()))
            .WithMessage($"Currency must be one of: {string.Join(", ", AllowedCurrencies)}.");
    }
}