using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface ICategoryRepository
{
    /// <summary>
    /// Returns system categories and the user's personal categories.
    /// </summary>
    Task<IEnumerable<Category>> GetAllForUserAsync(Guid userId, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsForUserAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task AddAsync(Category category, CancellationToken ct = default);
    Task DeleteAsync(Category category, CancellationToken ct = default);
    Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}