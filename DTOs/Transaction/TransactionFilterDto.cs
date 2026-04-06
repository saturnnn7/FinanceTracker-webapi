namespace FinanceTracker.DTOs.Transaction;

/// <summary>
/// Filter parameters for GET /api/transactions.
/// All fields are optional—we only pass what is needed.
/// </summary>
public class TransactionFilterDto
{
    public Guid?        AccountId   { get; set; }
    public Guid?        CategoryId  { get; set; }
    public DateTime?    DateFrom    { get; set; }
    public DateTime?    DateTo      { get; set; }
    public string?      Type        { get; set; }

    public int Page     { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}