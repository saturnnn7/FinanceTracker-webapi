namespace FinanceTracker.Models.Enums;

/// <summary>
/// Frequency of a recurring transaction.
/// Used in RecurringTransaction to determine when to create the next transaction.
/// </summary>
public enum RecurrenceInterval
{
    Daily   = 0,
    Weekly  = 1,
    Monthly = 2,
    Yearly  = 3
}