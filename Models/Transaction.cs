using FinanceTracker.Models.Interfaces;
using FinanceTracker.Models.Enums;

namespace FinanceTracker.Models;

/// <summary>
/// Two transactions are created for the transfer:
///   - Expense transaction on the source account (the TransferPairId points to the second one)
///   - Income transaction on the destination account (the TransferPairId points to the first one)
/// Both transactions are linked via the TransferPairId.
/// </summary>
public class Transaction : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Note { get; set; }

    /// <summary>Transaction date (not the date the record was created).</summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The ID of the second transaction in an inter-account transfer.
    /// Null for regular Income/Expense transactions.
    /// </summary>
    public Guid? TransferPairId { get; set; }

    /// <summary>
    /// A reference to the regular transaction, if it was created automatically.
    /// Null for manually created transactions.
    /// </summary>
    public Guid? RecurringTransactionId { get; set; }

    /// <summary>
    /// A link to the goal if the transaction is marked as a contribution to the goal.
    /// </summary>
    public Guid? GoalId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // FK
    public Guid AccountId { get; set; }
    public Guid CategoryId { get; set; }

    // Navigation properties
    public Account                  Account                 { get; set; } = null!;
    public Category                 Category                { get; set; } = null!;
    public RecurringTransaction?    RecurringTransaction    { get; set; }
    public Goal?                    Goal                    { get; set; }
}