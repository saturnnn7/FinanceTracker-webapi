using FinanceTracker.Models.Interfaces;

namespace FinanceTracker.Models;

/// <summary>
/// In-app user notification.
/// A background service is created automatically.
/// </summary>
public class Notification : IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "BudgetWarning", "RecurringReminder"
    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // FK
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}