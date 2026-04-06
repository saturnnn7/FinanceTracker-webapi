namespace FinanceTracker.DTOs.Transaction;

public class UpdateTransactionDto
{
    public decimal  Amount      { get; set; }
    public Guid     CategoryId  { get; set; }
    public DateTime Date        { get; set; }
    public string?  Note        { get; set; }
}