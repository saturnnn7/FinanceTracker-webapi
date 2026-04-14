using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.BackgroundServices;

/// <summary>
/// Background service — launches when the application starts.
/// Every 60 minutes, it checks for active recurring transaction templates
/// and creates transactions for which the NextRunAt time has arrived.
/// </summary>
public class RecurringTransactionProcessor : BackgroundService
{
    // How often do we check the database
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);

    // IServiceScopeFactory is needed because DbContext is a scoped service,
    // while BackgroundService is a singleton. You cannot inject a scoped service directly into a singleton.
    // Solution: Create a new scope for each iteration of the loop.
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RecurringTransactionProcessor> _logger;

    public RecurringTransactionProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<RecurringTransactionProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringTransactionProcessor started.");

        // The loop continues until the application is stopped
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueTransactionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the service —
                // a single error shouldn't kill the background processing
                _logger.LogError(ex, "Error processing recurring transactions.");
            }

            // We're looking forward to the next iteration
            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("RecurringTransactionProcessor stopped.");
    }

    // -------------------------------------------------------

    private async Task ProcessDueTransactionsAsync(CancellationToken ct)
    {
        // We create a new scope for each iteration
        using var scope   = _scopeFactory.CreateScope();
        var context       = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;

        // Find all active templates that are due
        var dueRecurring = await context.RecurringTransactions
            .Include(r => r.Account)
            .Include(r => r.Category)
            .Where(r => r.IsActive && r.NextRunAt <= now)
            .ToListAsync(ct);

        if (!dueRecurring.Any())
        {
            _logger.LogDebug("No recurring transactions due at {Time}.", now);
            return;
        }

        _logger.LogInformation(
            "Processing {Count} recurring transactions.", dueRecurring.Count);

        foreach (var recurring in dueRecurring)
        {
            await CreateTransactionFromTemplateAsync(context, recurring, now, ct);
        }

        await context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Successfully processed {Count} recurring transactions.", dueRecurring.Count);
    }

    private static async Task CreateTransactionFromTemplateAsync(
        AppDbContext context,
        RecurringTransaction recurring,
        DateTime now,
        CancellationToken ct)
    {
        // Creating a transaction based on a template
        var transaction = new Transaction
        {
            Amount                 = recurring.Amount,
            Type                   = recurring.Type,
            AccountId              = recurring.AccountId,
            CategoryId             = recurring.CategoryId,
            Date                   = now,
            Note                   = $"Auto: {recurring.Title}",
            RecurringTransactionId = recurring.Id
        };

        await context.Transactions.AddAsync(transaction, ct);

        // Update the template—set the next launch date
        recurring.LastRunAt = now;
        recurring.NextRunAt = CalculateNextRunAt(now, recurring.Interval);

        context.RecurringTransactions.Update(recurring);
    }

    // -------------------------------------------------------

    /// <summary>
    /// Calculates the next start date based on the interval.
    /// </summary>
    private static DateTime CalculateNextRunAt(DateTime from, RecurrenceInterval interval)
        => interval switch
        {
            RecurrenceInterval.Daily   => from.AddDays(1),
            RecurrenceInterval.Weekly  => from.AddDays(7),
            RecurrenceInterval.Monthly => from.AddMonths(1),
            RecurrenceInterval.Yearly  => from.AddYears(1),
            _                          => from.AddMonths(1)
        };
}