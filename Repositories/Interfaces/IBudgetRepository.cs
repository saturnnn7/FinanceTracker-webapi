using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Budget>> GetAllByUserAsync(Guid userId, int month, int year, CancellationToken ct = default);

    /// <summary>
    /// Checks whether a budget has already been allocated for this category this month.
    /// </summary>
    Task<bool> ExistsAsync(Guid userId, Guid categoryId, int month, int year, CancellationToken ct = default);
    Task AddAsync(Budget budget, CancellationToken ct = default);
    Task UpdateAsync(Budget budget, CancellationToken ct = default);
    Task DeleteAsync(Budget budget, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}