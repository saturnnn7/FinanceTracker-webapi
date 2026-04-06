using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface IGoalRepository
{
    Task<Goal?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Goal>> GetAllByUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task AddAsync(Goal goal, CancellationToken ct = default);
    Task UpdateAsync(Goal goal, CancellationToken ct = default);
    Task DeleteAsync(Goal goal, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}