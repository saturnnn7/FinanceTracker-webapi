namespace FinanceTracker.DTOs.Statistics;

/// <summary>Balance as of a specific date — for the trend chart.</summary>
public class BalanceHistoryDto
{
    public DateTime Date    { get; set; }
    public decimal  Balance { get; set; }
}