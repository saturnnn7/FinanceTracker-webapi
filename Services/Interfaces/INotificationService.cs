using FinanceTracker.Common;
using FinanceTracker.DTOs.Goal;
using FinanceTracker.DTOs.Notification;

namespace FinanceTracker.Services.Interfaces;

public interface INotificationService
{
    Task<Result<IEnumerable<NotificationResponseDto>>> GetAllAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
    Task<Result> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
}