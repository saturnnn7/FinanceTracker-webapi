using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly AppDbContext _context;

    public GoalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Goal?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Goals
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId, ct);

    public async Task<IEnumerable<Goal>> GetAllByUserAsync(
        Guid userId, CancellationToken ct = default)
        => await _context.Goals
            .Where(g => g.UserId == userId)
            .OrderBy(g => g.TargetDate)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Goals
            .AnyAsync(g => g.Id == id && g.UserId == userId, ct);

    public async Task AddAsync(Goal goal, CancellationToken ct = default)
        => await _context.Goals.AddAsync(goal, ct);

    public Task UpdateAsync(Goal goal, CancellationToken ct = default)
    {
        _context.Goals.Update(goal);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Goal goal, CancellationToken ct = default)
    {
        _context.Goals.Remove(goal);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}