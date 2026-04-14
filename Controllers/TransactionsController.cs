using FinanceTracker.DTOs.Transaction;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using CsvHelper;
using System.Globalization;

namespace FinanceTracker.Controllers;

[Route("api/transactions")]
[Authorize]
public class TransactionsController : BaseController
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Get transactions with filtering and pagination.
    /// All filter parameters are optional.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<TransactionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] TransactionFilterDto filter,
        CancellationToken ct)
    {
        var result = await _transactionService.GetAllAsync(GetUserId(), filter, ct);
        return FromResult(result);
    }

    /// <summary>Get transaction by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                 StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _transactionService.GetByIdAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>
    /// Create a transaction. For Transfer — provide ToAccountId.
    /// Transfer automatically creates two linked transactions.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),                 StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>),                 StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTransactionDto dto,
        CancellationToken ct)
    {
        var result = await _transactionService.CreateAsync(GetUserId(), dto, ct);

        if (result.IsFailure)
            return FromResult(result);

        return CreatedResponse(nameof(GetById), new { id = result.Value!.Id }, result.Value!);
    }

    /// <summary>
    /// Update transaction. Transfer transactions cannot be edited —
    /// delete and recreate instead.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                 StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>),                 StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody]  UpdateTransactionDto dto,
        CancellationToken ct)
    {
        var result = await _transactionService.UpdateAsync(id, GetUserId(), dto, ct);
        return FromResult(result);
    }

    /// <summary>
    /// Delete transaction. For Transfer — both linked transactions are deleted.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _transactionService.DeleteAsync(id, GetUserId(), ct);

        if (result.IsFailure)
            return FromResult(result);

        return NoContent();
    }

    /// <summary>
    /// Export transactions to CSV file for a date range.
    /// </summary>
    [HttpGet("export")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo,
        CancellationToken ct)
    {
        if (dateFrom > dateTo)
            return BadRequest(ApiResponse<object>.Fail(
                ErrorCodes.ValidationError,
                "dateFrom must be earlier than dateTo."));

        var result = await _transactionService.GetForExportAsync(
            GetUserId(), dateFrom, dateTo, ct);

        if (result.IsFailure)
            return FromResult(result);

        // Writing CSV to memory—not to disk
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, leaveOpen: true);
        var csv    = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(result.Value!, ct);
        await writer.FlushAsync(ct);
        stream.Position = 0;

        var fileName = $"transactions_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}.csv";

        // When you upload a file, the browser will prompt you to download it
        return File(stream, "text/csv", fileName);
    }
}