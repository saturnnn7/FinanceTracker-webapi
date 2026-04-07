using FinanceTracker.Common;
using FinanceTracker.DTOs.RecurringTransaction;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class RecurringTransactionService : IRecurringTransactionService
{
    private readonly IRecurringTransactionRepository _recurringRepository;
    private readonly IAccountRepository              _accountRepository;
    private readonly ICategoryRepository             _categoryRepository;
    private readonly ILogger<RecurringTransactionService> _logger;

    public RecurringTransactionService(
        IRecurringTransactionRepository recurringRepository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        ILogger<RecurringTransactionService> logger)
    {
        _recurringRepository = recurringRepository;
        _accountRepository   = accountRepository;
        _categoryRepository  = categoryRepository;
        _logger              = logger;
    }

    public async Task<Result<IEnumerable<RecurringTransactionResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default)
    {
        var items = await _recurringRepository.GetAllByUserAsync(userId, ct);
        return Result<IEnumerable<RecurringTransactionResponseDto>>.Ok(
            items.Select(MapToDto));
    }

    public async Task<Result<RecurringTransactionResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var item = await _recurringRepository.GetByIdAsync(id, userId, ct);
        if (item is null)
            return Result<RecurringTransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Recurring transaction not found.");

        return Result<RecurringTransactionResponseDto>.Ok(MapToDto(item));
    }

    public async Task<Result<RecurringTransactionResponseDto>> CreateAsync(
        Guid userId, CreateRecurringTransactionDto dto, CancellationToken ct = default)
    {
        if (!await _accountRepository.ExistsAsync(dto.AccountId, userId, ct))
            return Result<RecurringTransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Account not found.");

        if (!await _categoryRepository.ExistsForUserAsync(dto.CategoryId, userId, ct))
            return Result<RecurringTransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Category not found.");

        var interval = Enum.Parse<RecurrenceInterval>(dto.Interval, true);
        var type     = Enum.Parse<TransactionType>(dto.Type, true);

        var recurring = new RecurringTransaction
        {
            Title      = dto.Title,
            Amount     = dto.Amount,
            Type       = type,
            Interval   = interval,
            AccountId  = dto.AccountId,
            CategoryId = dto.CategoryId,
            UserId     = userId,
            NextRunAt  = dto.StartDate.ToUniversalTime()
        };

        await _recurringRepository.AddAsync(recurring, ct);
        await _recurringRepository.SaveChangesAsync(ct);

        var created = await _recurringRepository.GetByIdAsync(recurring.Id, userId, ct);
        return Result<RecurringTransactionResponseDto>.Ok(MapToDto(created!));
    }

    public async Task<Result<RecurringTransactionResponseDto>> ToggleActiveAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var item = await _recurringRepository.GetByIdAsync(id, userId, ct);
        if (item is null)
            return Result<RecurringTransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Recurring transaction not found.");

        item.IsActive = !item.IsActive;

        await _recurringRepository.UpdateAsync(item, ct);
        await _recurringRepository.SaveChangesAsync(ct);

        return Result<RecurringTransactionResponseDto>.Ok(MapToDto(item));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var item = await _recurringRepository.GetByIdAsync(id, userId, ct);
        if (item is null)
            return Result.Fail(ErrorCodes.NotFound, "Recurring transaction not found.");

        await _recurringRepository.DeleteAsync(item, ct);
        await _recurringRepository.SaveChangesAsync(ct);

        return Result.Ok();
    }

    // -------------------------------------------------------

    private static RecurringTransactionResponseDto MapToDto(RecurringTransaction r) => new()
    {
        Id           = r.Id,
        Title        = r.Title,
        Amount       = r.Amount,
        Type         = r.Type.ToString(),
        AccountName  = r.Account.Name,
        CategoryName = r.Category.Name,
        Interval     = r.Interval.ToString(),
        IsActive     = r.IsActive,
        NextRunAt    = r.NextRunAt,
        LastRunAt    = r.LastRunAt
    };
}