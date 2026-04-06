using FinanceTracker.DTOs.Transaction;
using FluentValidation;

namespace FinanceTracker.Validators.Transaction;

public class UpdateTransactionDtoValidator : AbstractValidator<UpdateTransactionDto>
{
    public UpdateTransactionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Date cannot be in the future.");
    }
}