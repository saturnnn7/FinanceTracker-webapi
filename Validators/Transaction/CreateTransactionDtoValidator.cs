using FinanceTracker.DTOs.Transaction;
using FinanceTracker.Models.Enums;
using FluentValidation;

namespace FinanceTracker.Validators.Transaction;

public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<TransactionType>(t, true, out _))
            .WithMessage($"Type must be one of: {string.Join(", ", Enum.GetNames<TransactionType>())}.");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("AccountId is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Date cannot be in the future.");

        // ToAccountId is required only for Transfer
        RuleFor(x => x.ToAccountId)
            .NotEmpty().WithMessage("ToAccountId is required for Transfer.")
            .When(x => x.Type?.Equals("Transfer", StringComparison.OrdinalIgnoreCase) == true);

        // ToAccountId must be different from AccountId
        RuleFor(x => x.ToAccountId)
            .NotEqual(x => x.AccountId)
            .WithMessage("Transfer source and destination accounts must be different.")
            .When(x => x.ToAccountId.HasValue);
    }
}