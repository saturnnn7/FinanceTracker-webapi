using FinanceTracker.DTOs.Budget;
using FluentValidation;

namespace FinanceTracker.Validators.Budget;

public class CreateBudgetDtoValidator : AbstractValidator<CreateBudgetDto>
{
    public CreateBudgetDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.LimitAmount)
            .GreaterThan(0).WithMessage("Limit amount must be greater than 0.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100).WithMessage("Year must be between 2000 and 2100.");
    }
}