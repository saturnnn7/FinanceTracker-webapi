using System.Diagnostics.CodeAnalysis;
using FinanceTracker.Common;
using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace FinanceTracker.BackgroundServices;

/// <summary>
/// Background service — checks budgets and recurring transactions
/// and generates notifications for users.
/// Runs every 6 hours.
/// </summary>
public class NotificationProcessor : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<NotificationProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationProcessor started.");

        // First start
        await ProcessAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_interval, stoppingToken);
                await ProcessAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the service —
                // a single error shouldn't kill the background processing
                _logger.LogError(ex, "Error in NotificationProcessor.");
            }
        }
    }

    // -------------------------------------------

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await CheckBudgetsAsync(context, ct);
        await CheckRecurringTransactionsAsync(context, ct);

        await context.SaveChangesAsync(ct);
    }

    // -------------------------------------------

    private async Task CheckBudgetsAsync(AppDbContext context, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var month = now.Month;
        var year = now.Year;

        var budgets = await context.Budgets
            .Include(b => b.Category)
            .Where(b => b.Month == month && b.Year == year)
            .ToListAsync(ct);

        foreach(var budget in budgets)
        {
            var spentAmount = await context.Transactions
                .Where(t =>
                    t.Account.UserId    == budget.UserId &&
                    t.CategoryId        == budget.CategoryId &&
                    t.Type              == TransactionType.Expense &&
                    t.Date.Month        == month &&
                    t.Date.Year         == year)
                .Select(t => t.Amount)
                .ToListAsync(ct);
            
            var spent = spentAmount.Sum();
            var percent = budget.LimitAmount == 0 ? 0 : spent / budget.LimitAmount * 100;

            if(percent >= 100)
            {
                await CreateIfNotExistsAsync(context,
                    budget.UserId,
                    NotificationTypes.BudgetExceeded,
                    $"Budget exceeded: {budget.Category.Name}",
                    $"You have spent {spent:F2} of {budget.LimitAmount:F2} " + 
                    $"({percent:F0}%) in {budget.Category.Name} this month.",
                    ct);
            }
            else if (percent >= 80)
            {
                await CreateIfNotExistsAsync(context,
                    budget.UserId,
                    NotificationTypes.BudgetWarning,
                    $"Budget warning: {budget.Category.Name}",
                    $"You have spent {spent:F2} of {budget.LimitAmount:F2} " + 
                    $"({percent:F0}%) in {budget.Category.Name} this month.",
                    ct);
            }
        }
    }

    private async Task CheckRecurringTransactionsAsync(AppDbContext context, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var tomorrow = now.AddHours(24);

        var upcoming = await context.RecurringTransactions
            .Include(r => r.Account)
            .Where(r => 
                r.IsActive          &&
                r.NextRunAt > now   &&
                r.NextRunAt <= tomorrow)
            .ToListAsync(ct);
    
        foreach(var recurring in upcoming)
        {
            await CreateIfNotExistsAsync(context,
                recurring.UserId,
                NotificationTypes.RecurringReminder,
                $"Upcoming: {recurring.Title}",
                $"{recurring.Amount:F2} will be charged from " +
                $"{recurring.Account.Name} in less than 24 hours.",
                ct);
        }
    }

    // -------------------------------------------
    
    private async Task CreateIfNotExistsAsync(
        AppDbContext context,
        Guid userId,
        string type,
        string title,
        string message,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;

        var exists = await context.Notifications.AnyAsync(n =>
            n.UserId == userId &&
            n.Type == type &&
            n.Title == title &&
            n.CreatedAt >= today,
            ct);
        if (exists) return;

        await context.Notifications.AddAsync(new Notification
        {
           UserId = userId,
           Type = type,
           Title = title,
           Message = message 
        }, ct);

        _logger.LogInformation("Notification created for user {UserId}: {Title}", userId, title);
    }
}