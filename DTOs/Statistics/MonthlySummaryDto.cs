namespace FinanceTracker.DTOs.Statistics;

/// <summary>Monthly Summary.</summary>
public class MonthlySummaryDto
{
    public int     Month        { get; set; }
    public int     Year         { get; set; }
    public decimal TotalIncome  { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Net          => TotalIncome - TotalExpense;
}