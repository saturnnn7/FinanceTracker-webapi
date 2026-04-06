namespace FinanceTracker.DTOs.RecurringTransaction;

public class RecurringTransactionResponseDto
{
    public Guid     Id           { get; set; }
    public string   Title        { get; set; } = string.Empty;
    public decimal  Amount       { get; set; }
    public string   Type         { get; set; } = string.Empty;
    public string   AccountName  { get; set; } = string.Empty;
    public string   CategoryName { get; set; } = string.Empty;
    public string   Interval     { get; set; } = string.Empty;
    public bool     IsActive     { get; set; }
    public DateTime NextRunAt    { get; set; }
    public DateTime? LastRunAt   { get; set; }
}