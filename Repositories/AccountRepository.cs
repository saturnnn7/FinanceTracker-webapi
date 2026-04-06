using FinanceTracker.Models;
using FinanceTracker.Data;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;
    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, ct);

    public async Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId, CancellationToken ct = default)
        => await _context.Accounts
            .Where(a => a.UserId == userId && !a.IsArchived)
            .OrderBy(a => a.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Accounts
            .AnyAsync(a => a.Id == id && a.UserId == userId, ct);

    public Task AddAsync(Account account, CancellationToken ct = default)
    {
        _context.Accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        _context.Accounts.Update(account);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Account account, CancellationToken ct = default)
    {
        _context.Accounts.Remove(account);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}