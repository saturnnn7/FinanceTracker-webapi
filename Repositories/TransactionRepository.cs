using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Data;
using FinanceTracker.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _context;
    public TransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        => await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id && t.Account.UserId == userId, ct);

    public async Task<IEnumerable<Transaction>> GetAllAsync(
        Guid userId,
        Guid? accountId,
        Guid? categoryId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? type,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .Where(t => t.Account.UserId == userId);

        // Фильтры — применяем только если переданы
        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (dateFrom.HasValue)
            query = query.Where(t => t.Date >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(t => t.Date <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(type) &&
            Enum.TryParse<TransactionType>(type, true, out var transactionType))
            query = query.Where(t => t.Type == transactionType);

        return await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        Guid userId,
        Guid? accountId,
        Guid? categoryId,
        DateTime? dateFrom,
        DateTime? dateTo,
        string? type,
        CancellationToken ct = default)
    {
        var query = _context.Transactions
            .Where(t => t.Account.UserId == userId);

        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (dateFrom.HasValue)
            query = query.Where(t => t.Date >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(t => t.Date <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(type) &&
            Enum.TryParse<TransactionType>(type, true, out var transactionType))
            query = query.Where(t => t.Type == transactionType);

        return await query.CountAsync(ct);
    }

    public async Task<decimal> GetBalanceByAccountAsync(Guid accountId, CancellationToken ct = default)
    {
        // Считаем баланс как SUM(Income) - SUM(Expense) прямо в БД
        // Transfer не участвует — он уже разбит на Income + Expense
        var income = await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount, ct);

        var expense = await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount, ct);

        return income - expense;
    }

    public async Task<decimal> GetSpentByCategoryAsync(
        Guid userId,
        Guid categoryId,
        int month,
        int year,
        CancellationToken ct = default)
        => await _context.Transactions
            .Where(t =>
                t.Account.UserId == userId &&
                t.CategoryId == categoryId &&
                t.Type == TransactionType.Expense &&
                t.Date.Month == month &&
                t.Date.Year == year)
            .SumAsync(t => t.Amount, ct);

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        => await _context.Transactions.AddAsync(transaction, ct);

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default)
        => await _context.Transactions.AddRangeAsync(transactions, ct);

    public Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _context.Transactions.Update(transaction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Transaction transaction, CancellationToken ct = default)
    {
        _context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}