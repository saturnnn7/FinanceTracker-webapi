using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Filtering + pagination. All parameters are optional.
    /// </summary>
    Task<IEnumerable<Transaction>> GetAllAsync(
        Guid userId, Guid? accountId, Guid? categoryId,
        DateTime? dateFrom, DateTime? dateTo,
        string? type, int page, int pageSize,
        CancellationToken ct = default);

    Task<int> CountAsync(
        Guid userId, Guid? accountId, Guid? categoryId,
        DateTime? dateFrom, DateTime? dateTo,
        string? type,
        CancellationToken ct = default);

    /// <summary>
    /// Total account transactions — used to calculate the balance.
    /// </summary>
    Task<decimal> GetBalanceByAccountAsync(Guid accountId, CancellationToken ct = default);

    /// <summary>
    /// Total expenses by category for the period — for budget tracking.
    /// </summary>
    Task<decimal> GetSpentByCategoryAsync(
        Guid userId, Guid categoryId,
        int month, int year,
        CancellationToken ct = default);

    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Transaction> transaction, CancellationToken ct = default);
    Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    Task DeleteAsync(Transaction transaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

