using FinanceTracker.Common;
using FinanceTracker.DTOs.Goal;
using FinanceTracker.DTOs.Notification;
using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    public NotificationService(INotificationRepository notificationRepository)
    {
       _notificationRepository = notificationRepository;
    }

    public async Task<Result<IEnumerable<NotificationResponseDto>>> GetAllAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 50);
        page = Math.Max(page, 1);

        var notifications = await _notificationRepository
            .GetAllByUserAsync(userId, page, pageSize, ct);
        
        return Result<IEnumerable<NotificationResponseDto>>.Ok(
            notifications.Select(MapToDto));
    }

    public async Task<Result<int>> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
    {
        var count = await _notificationRepository.GetUnreadCountAsync(userId, ct);
        return Result<int>.Ok(count);
    }

    public async Task<Result> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, ct);
        return Result.Ok();
    }

    // --------------------------------------------

    private static NotificationResponseDto MapToDto(Notification n) => new()
    {
        Id          = n.Id,
        Title       = n.Title,
        Message     = n.Message,
        Type        = n.Type,
        IsRead      = n.IsRead,
        CreatedAt   = n.CreatedAt,
    };
}