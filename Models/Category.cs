using FinanceTracker.Models.Interfaces;

namespace FinanceTracker.Models;

/// <summary>
/// Transaction category (Food, Transportation, Salary, etc.)
/// IsSystem = true — created automatically upon registration; cannot be deleted.
/// IsSystem = false — user-defined; can be deleted if there are no transactions.
/// </summary>
public class Category : IAuditableEntity
{
    public Guid     Id          { get; set; } = Guid.NewGuid();
    public string   Name        { get; set; } = string.Empty;
    public string   Icon        { get; set; } = "📦"; // Emoji icon for the UI
    public string   Color       { get; set; } = "#6C757D";
    public bool     IsSystem    { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // FK — null for system categories
    public Guid? UserId { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}