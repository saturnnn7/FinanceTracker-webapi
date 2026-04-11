using FinanceTracker.DTOs.Statistics;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/statistics")]
[Authorize]
public class StatisticsController : BaseController
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Expenses breakdown by category for a date range.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryStatDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryStats(
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo,
        CancellationToken ct)
    {
        var result = await _statisticsService
            .GetCategoryStatsAsync(GetUserId(), dateFrom, dateTo, ct);
        return FromResult(result);
    }

    /// <summary>
    /// Monthly income vs expense summary for a given year.
    /// </summary>
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MonthlySummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlySummary(
        [FromQuery] int year,
        CancellationToken ct)
    {
        var targetYear = year == 0 ? DateTime.UtcNow.Year : year;

        var result = await _statisticsService
            .GetMonthlySummaryAsync(GetUserId(), targetYear, ct);
        return FromResult(result);
    }

    /// <summary>
    /// Account balance history by day — for chart visualization.
    /// </summary>
    [HttpGet("balance-history")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BalanceHistoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                         StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBalanceHistory(
        [FromQuery] Guid     accountId,
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo,
        CancellationToken ct)
    {
        var result = await _statisticsService
            .GetBalanceHistoryAsync(GetUserId(), accountId, dateFrom, dateTo, ct);
        return FromResult(result);
    }
}