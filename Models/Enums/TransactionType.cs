namespace FinanceTracker.Models.Enums;

/// <summary>
/// Transaction type.
/// Transfer is a special case: it creates two linked transactions simultaneously.
/// </summary>
public enum TransactionType
{
    Income      = 0,
    Expense     = 1,
    Transfer    = 2
}