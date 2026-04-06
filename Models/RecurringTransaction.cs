using FinanceTracker.Models.Interfaces;
using FinanceTracker.Models.Enums;

namespace FinanceTracker.Models;

/// <summary>
/// Template for a recurring transaction (subscription, rental, payroll).
/// The Background Service checks NextRunAt and creates the transaction automatically.
/// </summary>
public class RecurringTransaction : IAuditableEntity
{
    public Guid                 Id          { get; set; } = Guid.NewGuid();
    public string               Title       { get; set; } = string.Empty;
    public decimal              Amount      { get; set; }
    public TransactionType      Type        { get; set; }
    public RecurrenceInterval   Interval    { get; set; }
    public bool                 IsActice    { get; set; } = true;

    /// <summary>The date of the next automatic transaction creation.</summary>
    public DateTime NextRunAt   { get; set; }

    /// <summary>The date the transaction was last created. Null if it has not yet been started.</summary>
    public DateTime? LastRunAt  { get; set; }

    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }

    // FK
    public Guid UserId      { get; set; }
    public Guid AccountId   { get; set; }
    public Guid CategoryId  { get; set; }

    // Navigation properties
    public User                     User            { get; set; } = null!;
    public Account                  Account         { get; set; } = null!;
    public Category                 Category        { get; set; } = null!;
    public ICollection<Transaction> Transactions    { get; set; } = new List<Transaction>();
}