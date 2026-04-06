using FinanceTracker.Models;

namespace FinanceTracker.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task AddAsync(Account account, CancellationToken ct = default);
    Task UpdateAsync(Account account, CancellationToken ct = default);
    Task DeleteAsync(Account account, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}