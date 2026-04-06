
using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class RecurringTransactionRepository : IRecurringTransactionRepository
{
    private readonly AppDbContext _context;

    public RecurringTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringTransaction?> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
        => await _context.RecurringTransactions
            .Include(r => r.Account)
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId, ct);

    public async Task<IEnumerable<RecurringTransaction>> GetAllByUserAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.RecurringTransactions
            .Include(r => r.Account)
            .Include(r => r.Category)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.NextRunAt)
            .ToListAsync(ct);

    /// <summary>
    /// Используется Background Service — ищет все активные у которых пришло время.
    /// </summary>
    public async Task<IEnumerable<RecurringTransaction>> GetDueAsync(
        DateTime now, CancellationToken ct = default)
        => await _context.RecurringTransactions
            .Include(r => r.Account)
            .Include(r => r.Category)
            .Where(r => r.IsActive && r.NextRunAt <= now)
            .ToListAsync(ct);

    public async Task AddAsync(RecurringTransaction recurring, CancellationToken ct = default)
        => await _context.RecurringTransactions.AddAsync(recurring, ct);

    public Task UpdateAsync(RecurringTransaction recurring, CancellationToken ct = default)
    {
        _context.RecurringTransactions.Update(recurring);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RecurringTransaction recurring, CancellationToken ct = default)
    {
        _context.RecurringTransactions.Remove(recurring);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}