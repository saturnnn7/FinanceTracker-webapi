using FinanceTracker.DTOs.Goal;
using FluentValidation;

namespace FinanceTracker.Validators.Goal;

public class CreateGoalDtoValidator : AbstractValidator<CreateGoalDto>
{
    public CreateGoalDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description != null);

        RuleFor(x => x.TargetAmount)
            .GreaterThan(0).WithMessage("Target amount must be greater than 0.");

        RuleFor(x => x.TargetDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Target date must be in the future.");
    }
}