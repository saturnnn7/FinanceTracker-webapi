using FinanceTracker.DTOs.RecurringTransaction;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/recurring-transactions")]
[Authorize]
public class RecurringTransactionsController : BaseController
{
    private readonly IRecurringTransactionService _recurringService;

    public RecurringTransactionsController(IRecurringTransactionService recurringService)
    {
        _recurringService = recurringService;
    }

    /// <summary>Get all recurring transaction templates</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RecurringTransactionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _recurringService.GetAllAsync(GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Get recurring transaction by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RecurringTransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                          StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _recurringService.GetByIdAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Create a recurring transaction template</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RecurringTransactionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),                          StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRecurringTransactionDto dto,
        CancellationToken ct)
    {
        var result = await _recurringService.CreateAsync(GetUserId(), dto, ct);

        if (result.IsFailure)
            return FromResult(result);

        return CreatedResponse(nameof(GetById), new { id = result.Value!.Id }, result.Value!);
    }

    /// <summary>Toggle active/inactive status</summary>
    [HttpPatch("{id:guid}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<RecurringTransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                          StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Toggle([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _recurringService.ToggleActiveAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Delete recurring transaction template</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _recurringService.DeleteAsync(id, GetUserId(), ct);

        if (result.IsFailure)
            return FromResult(result);

        return NoContent();
    }
}