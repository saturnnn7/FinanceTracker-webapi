namespace FinanceTracker.Models.Enums;

/// <summary>
/// User account type.
/// This only affects the display - the business logic is the same for all types.
/// </summary>
public enum AccountType
{
    Cash    = 0,
    Card    = 1, // Debit or credit card
    Deposit = 2,
    Credit  = 3,
    Other   = 4
}