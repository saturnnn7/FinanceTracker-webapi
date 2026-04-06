using FinanceTracker.Models.Interfaces;
using FinanceTracker.Models.Enums;

namespace FinanceTracker.Models;

public class Account : IAuditableEntity
{
    public Guid         Id      { get; set; } = Guid.NewGuid();
    public string       Name    { get; set; } = string.Empty;
    public AccountType  Type    { get; set; } = AccountType.Card;

    /// <summary>
    /// Opening balance at the time the account was created.
    /// This is needed so you don't have to enter all past transactions manually.
    /// </summary>
    public decimal InitialBalance { get; set; } = 0;

    /// <summary>
    /// Account currency. ISO 4217 code: RUB, USD, EUR.
    /// </summary>
    public string Currency { get; set; } = "PLN";

    /// <summary>
    /// Color to display in the UI. Stored as a HEX string: “#FF5733”
    /// </summary>
    public string Color { get; set; } = "#6C757D";

    /// <summary>
    /// An archived account does not appear in the main list,
    /// but its transactions are retained for historical purposes.
    /// </summary>
    public bool IsArchived { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // FK
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}