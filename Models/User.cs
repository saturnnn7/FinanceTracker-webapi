using FinanceTracker.Models.Interfaces;

namespace FinanceTracker.Models;

/// <summary>
/// System user.
/// All data (accounts, transactions, categories) is strictly isolated by UserId.
/// </summary>
public class User : IAuditableEntity
{
    public Guid     Id              { get; set; } = Guid.NewGuid();
    public string   Username        { get; set; } = string.Empty;
    public string   Email           { get; set; } = string.Empty;
    public string   PasswordHash    { get; set; } = string.Empty;

    /// <summary>
    /// The user's primary currency.
    /// Statistics are converted to this currency for multi-currency accounts.
    /// ISO 4217 code: RUB, USD, EUR, etc.
    /// </summary>
    public string BaseCurrency { get; set; } = "PLN";

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // Navigation properties
    public ICollection<Account>                 Accounts                { get; set; } = new List<Account>();
    public ICollection<Category>                Categories              { get; set; } = new List<Category>();
    public ICollection<Budget>                  Budgets                 { get; set; } = new List<Budget>();
    public ICollection<RecurringTransaction>    RecurringTransactions   { get; set; } = new List<RecurringTransaction>();
    public ICollection<Goal>                    Goals                   { get; set; } = new List<Goal>();
}