using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetAllByUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);

    Task<IEnumerable<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken ct = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);

    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}