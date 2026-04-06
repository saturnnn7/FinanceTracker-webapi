using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface IRecurringTransactionRepository
{
    Task<RecurringTransaction?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<RecurringTransaction>> GetAllByUserAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Returns active templates for which NextRunAt is less than or equal to now.
    /// Uses the Background Service.
    /// </summary>
    Task<IEnumerable<RecurringTransaction>> GetDueAsync(DateTime now, CancellationToken ct = default);

    Task AddAsync(RecurringTransaction recurringTransaction, CancellationToken ct = default);
    Task UpdateAsync(RecurringTransaction recurringTransaction, CancellationToken ct = default);
    Task DeleteAsync(RecurringTransaction recurringTransaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

