using FinanceTracker.Common;
using FinanceTracker.Data;
using FinanceTracker.DTOs.Statistics;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models.Enums;
using FinanceTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public class StatisticsService : IStatisticsService
{
    private readonly AppDbContext _context;
    private readonly ILogger<StatisticsService> _logger;

    // StatisticsService works directly with DbContext —
    // complex aggregations are easier to write using LINQ than through a repository
    public StatisticsService(AppDbContext context, ILogger<StatisticsService> logger)
    {
        _context = context;
        _logger  = logger;
    }

    public async Task<Result<IEnumerable<CategoryStatDto>>> GetCategoryStatsAsync(
        Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken ct = default)
    {
        // Group expenses by category for the period
        var stats = await _context.Transactions
            .Where(t =>
                t.Account.UserId == userId &&
                t.Type == TransactionType.Expense &&
                t.Date >= dateFrom &&
                t.Date <= dateTo)
            .GroupBy(t => new
            {
                t.CategoryId,
                t.Category.Name,
                t.Category.Icon,
                t.Category.Color
            })
            .Select(g => new
            {
                g.Key.CategoryId,
                g.Key.Name,
                g.Key.Icon,
                g.Key.Color,
                Amount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .ToListAsync(ct);

        var total = stats.Sum(s => s.Amount);

        var dtos = stats.Select(s => new CategoryStatDto
        {
            CategoryId   = s.CategoryId,
            CategoryName = s.Name,
            CategoryIcon = s.Icon,
            Color        = s.Color,
            Amount       = s.Amount,
            // Percentage of total expenses for the period
            Percent      = total == 0 ? 0 : Math.Round(s.Amount / total * 100, 1)
        });

        return Result<IEnumerable<CategoryStatDto>>.Ok(dtos);
    }

    public async Task<Result<IEnumerable<MonthlySummaryDto>>> GetMonthlySummaryAsync(
        Guid userId, int year, CancellationToken ct = default)
    {
        var data = await _context.Transactions
            .Where(t => t.Account.UserId == userId && t.Date.Year == year)
            .GroupBy(t => new { t.Date.Month, t.Type })
            .Select(g => new
            {
                g.Key.Month,
                g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync(ct);

        // We compile a summary for each month
        var summary = Enumerable.Range(1, 12).Select(month => new MonthlySummaryDto
        {
            Month        = month,
            Year         = year,
            TotalIncome  = data
                .Where(d => d.Month == month && d.Type == TransactionType.Income)
                .Sum(d => d.Total),
            TotalExpense = data
                .Where(d => d.Month == month && d.Type == TransactionType.Expense)
                .Sum(d => d.Total)
        });

        return Result<IEnumerable<MonthlySummaryDto>>.Ok(summary);
    }

    public async Task<Result<IEnumerable<BalanceHistoryDto>>> GetBalanceHistoryAsync(
        Guid userId, Guid accountId, DateTime dateFrom, DateTime dateTo,
        CancellationToken ct = default)
    {
        // We verify that the account belongs to the user
        var account = await _context.Accounts
            .FirstOrDefaultAsync(
                a => a.Id == accountId && a.UserId == userId, ct);

        if (account is null)
            return Result<IEnumerable<BalanceHistoryDto>>.Fail(
                ErrorCodes.NotFound, "Account not found.");

        // Retrieve all transactions for the period, grouped by day
        var dailyChanges = await _context.Transactions
            .Where(t =>
                t.AccountId == accountId &&
                t.Date >= dateFrom &&
                t.Date <= dateTo)
            .GroupBy(t => t.Date.Date)
            .Select(g => new
            {
                Date   = g.Key,
                Change = g.Sum(t =>
                    t.Type == TransactionType.Income  ?  t.Amount :
                    t.Type == TransactionType.Expense ? -t.Amount : 0)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        // Opening balance — everything up to dateFrom
        var initialBalance = account.InitialBalance + await _context.Transactions
            .Where(t => t.AccountId == accountId && t.Date < dateFrom)
            .SumAsync(t =>
                t.Type == TransactionType.Income  ?  t.Amount :
                t.Type == TransactionType.Expense ? -t.Amount : 0, ct);

        // Building a history — cumulative total by day
        var history  = new List<BalanceHistoryDto>();
        var runningBalance = initialBalance;

        foreach (var day in dailyChanges)
        {
            runningBalance += day.Change;
            history.Add(new BalanceHistoryDto
            {
                Date    = day.Date,
                Balance = runningBalance
            });
        }

        return Result<IEnumerable<BalanceHistoryDto>>.Ok(history);
    }
}