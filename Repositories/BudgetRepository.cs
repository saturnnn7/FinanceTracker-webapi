using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _context;

    public BudgetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Budget?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId, ct);

    public async Task<IEnumerable<Budget>> GetAllByUserAsync(
        Guid userId, int month, int year, CancellationToken ct = default)
        => await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(
        Guid userId, Guid categoryId, int month, int year, CancellationToken ct = default)
        => await _context.Budgets
            .AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year, ct);

    public async Task AddAsync(Budget budget, CancellationToken ct = default)
        => await _context.Budgets.AddAsync(budget, ct);

    public Task UpdateAsync(Budget budget, CancellationToken ct = default)
    {
        _context.Budgets.Update(budget);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Budget budget, CancellationToken ct = default)
    {
        _context.Budgets.Remove(budget);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}