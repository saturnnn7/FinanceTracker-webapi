using FinanceTracker.Common;
using FinanceTracker.DTOs.Statistics;

namespace FinanceTracker.Services.Interfaces;

public interface IStatisticsService
{
    /// <summary>Expenses by category for the period.</summary>
    Task<Result<IEnumerable<CategoryStatDto>>> GetCategoryStatsAsync(
        Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken ct = default);

    /// <summary>Monthly summary for the year.</summary>
    Task<Result<IEnumerable<MonthlySummaryDto>>> GetMonthlySummaryAsync(
        Guid userId, int year, CancellationToken ct = default);

    /// <summary>Daily balance trends for the period.</summary>
    Task<Result<IEnumerable<BalanceHistoryDto>>> GetBalanceHistoryAsync(
        Guid userId, Guid accountId, DateTime dateFrom, DateTime dateTo, CancellationToken ct = default);
}