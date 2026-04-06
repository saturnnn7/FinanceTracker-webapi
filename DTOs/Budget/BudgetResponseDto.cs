namespace FinanceTracker.DTOs.Budget;

public class BudgetResponseDto
{
    public Guid     Id              { get; set; }
    public string   CategoryName    { get; set; } = string.Empty;
    public string   CategoryIcon    { get; set; } = string.Empty;
    public decimal  LimitAmount     { get; set; }

    /// <summary>The amount spent so far is calculated based on the transactions.</summary>
    public decimal  SpentAmount     { get; set; }

    /// <summary>Remaining: LimitAmount - SpentAmount.</summary>
    public decimal  Remaining       => LimitAmount - SpentAmount;

    /// <summary>Usage percentage (0–100).</summary>
    public decimal  UsagePercent    => LimitAmount == 0 ? 0
                                        : Math.Round(SpentAmount / LimitAmount * 100, 1);

    public int      Month           { get; set; }
    public int      Year            { get; set; }
}