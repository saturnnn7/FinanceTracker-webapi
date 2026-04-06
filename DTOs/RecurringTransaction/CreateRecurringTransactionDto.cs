namespace FinanceTracker.DTOs.RecurringTransaction;

public class CreateRecurringTransactionDto
{
    public string   Title      { get; set; } = string.Empty;
    public decimal  Amount     { get; set; }
    public string   Type       { get; set; } = string.Empty;
    public Guid     AccountId  { get; set; }
    public Guid     CategoryId { get; set; }
    public string   Interval   { get; set; } = string.Empty;  // "Daily", "Weekly", "Monthly", "Yearly"
    public DateTime StartDate  { get; set; }
}