using FinanceTracker.DTOs.RecurringTransaction;
using FinanceTracker.Models.Enums;
using FluentValidation;

namespace FinanceTracker.Validators.RecurringTransaction;

public class CreateRecurringTransactionDtoValidator
    : AbstractValidator<CreateRecurringTransactionDto>
{
    public CreateRecurringTransactionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100);

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

        RuleFor(x => x.Interval)
            .NotEmpty()
            .Must(i => Enum.TryParse<RecurrenceInterval>(i, true, out _))
            .WithMessage($"Interval must be one of: {string.Join(", ", Enum.GetNames<RecurrenceInterval>())}.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("StartDate is required.");
    }
}