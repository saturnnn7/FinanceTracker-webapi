

namespace FinanceTracker.DTOs.Notification;

public class NotificationResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "BudgetWarning", "RecurringReminder"
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }
}