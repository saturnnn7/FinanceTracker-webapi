using FinanceTracker.Models;
using FinanceTracker.Data;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// System users (UserId == null) + individual users.
    /// </summary>
    public async Task<IEnumerable<Category>> GetAllForUserAsync(Guid userId, CancellationToken ct = default)
        => await _context.Categories
            .Where(c => c.UserId == null || c.UserId == userId)
            .OrderBy(c => c.IsSystem)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Categories.FindAsync([id], ct);

    /// <summary>
    /// System categories are available to everyone—we check for `IsSystem` OR membership.
    /// </summary>
    public async Task<bool> ExistsForUserAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Categories
            .AnyAsync(c => c.Id == id && (c.IsSystem || c.UserId == userId), ct);

    public async Task AddAsync(Category category, CancellationToken ct = default)
        => await _context.Categories.AddAsync(category, ct);

    public Task DeleteAsync(Category category, CancellationToken ct = default)
    {
        _context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken ct = default)
        => await _context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}