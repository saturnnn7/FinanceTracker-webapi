namespace FinanceTracker.DTOs.Transaction;

public class CreateTransactionDto
{
    public decimal  Amount      { get; set; }
    public string   Type        { get; set; } = string.Empty;
    public Guid     AccountId   { get; set; }
    public Guid     CategoryId  { get; set; }
    public DateTime Date        { get; set; }
    public string?  Note        { get; set; }

    /// <summary>
    /// For Transfer only — the recipient's account ID.
    /// </summary>
    public Guid? ToAccountId { get; set; }

    /// <summary>
    /// Link a transaction to a savings goal.
    /// </summary>
    public Guid? GoalId { get; set; }
}