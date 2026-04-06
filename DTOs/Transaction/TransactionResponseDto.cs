using System.Security.AccessControl;

namespace FinanceTracker.DTOs.Transaction;

public class TransactionResponseDto
{
    public Guid     Id              { get; set; }
    public decimal  Amount          { get; set; }
    public string   Type            { get; set; } = string.Empty;
    public string   AccountName     { get; set; } = string.Empty;
    public string   CategoryName    { get; set; } = string.Empty;
    public string   CategoryIcon    { get; set; } = string.Empty;
    public DateTime Date            { get; set; }
    public string?  Note            { get; set; }
    public Guid?    TransferPairId  { get; set; }
    public Guid?    GoalId          { get; set; }
    public DateTime CreatedAt       { get; set; }
}