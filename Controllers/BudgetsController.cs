using FinanceTracker.DTOs.Budget;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/budgets")]
[Authorize]
public class BudgetsController : BaseController
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    /// <summary>Get budgets for a specific month and year</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BudgetResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken ct)
    {
        // If it hasn't been submitted, use this month's figures
        var targetMonth = month == 0 ? DateTime.UtcNow.Month : month;
        var targetYear  = year  == 0 ? DateTime.UtcNow.Year  : year;

        var result = await _budgetService.GetAllAsync(GetUserId(), targetMonth, targetYear, ct);
        return FromResult(result);
    }

    /// <summary>Get budget by id with current spent amount</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BudgetResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),            StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _budgetService.GetByIdAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Create a budget limit for a category and month</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BudgetResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),            StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>),            StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateBudgetDto dto, CancellationToken ct)
    {
        var result = await _budgetService.CreateAsync(GetUserId(), dto, ct);

        if (result.IsFailure)
            return FromResult(result);

        return CreatedResponse(nameof(GetById), new { id = result.Value!.Id }, result.Value!);
    }

    /// <summary>Update budget limit amount</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BudgetResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),            StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody]  UpdateBudgetDto dto,
        CancellationToken ct)
    {
        var result = await _budgetService.UpdateAsync(id, GetUserId(), dto, ct);
        return FromResult(result);
    }

    /// <summary>Delete budget</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _budgetService.DeleteAsync(id, GetUserId(), ct);

        if (result.IsFailure)
            return FromResult(result);

        return NoContent();
    }
}