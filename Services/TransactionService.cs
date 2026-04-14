using FinanceTracker.Common;
using FinanceTracker.DTOs.Common;
using FinanceTracker.DTOs.Transaction;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository     _accountRepository;
    private readonly ICategoryRepository    _categoryRepository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _accountRepository     = accountRepository;
        _categoryRepository    = categoryRepository;
        _logger                = logger;
    }

    public async Task<Result<PagedResponse<TransactionResponseDto>>> GetAllAsync(
        Guid userId, TransactionFilterDto filter, CancellationToken ct = default)
    {
        // Set a limit on pageSize
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        var page     = Math.Max(filter.Page, 1);

        var items = await _transactionRepository.GetAllAsync(
            userId, filter.AccountId, filter.CategoryId,
            filter.DateFrom, filter.DateTo, filter.Type,
            page, pageSize, ct);

        var total = await _transactionRepository.CountAsync(
            userId, filter.AccountId, filter.CategoryId,
            filter.DateFrom, filter.DateTo, filter.Type, ct);

        var response = new PagedResponse<TransactionResponseDto>
        {
            Items      = items.Select(MapToDto),
            TotalCount = total,
            Page       = page,
            PageSize   = pageSize
        };

        return Result<PagedResponse<TransactionResponseDto>>.Ok(response);
    }

    public async Task<Result<TransactionResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId, ct);
        if (transaction is null)
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Transaction not found.");

        return Result<TransactionResponseDto>.Ok(MapToDto(transaction));
    }

    public async Task<Result<TransactionResponseDto>> CreateAsync(
        Guid userId, CreateTransactionDto dto, CancellationToken ct = default)
    {
        // We verify that the account belongs to the user
        if (!await _accountRepository.ExistsAsync(dto.AccountId, userId, ct))
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Account not found.");

        // Checking if the category is available to the user
        if (!await _categoryRepository.ExistsForUserAsync(dto.CategoryId, userId, ct))
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Category not found.");

        var transactionType = Enum.Parse<TransactionType>(dto.Type, true);

        // Transfer is a special case: creating two transactions
        if (transactionType == TransactionType.Transfer)
            return await CreateTransferAsync(userId, dto, ct);

        var transaction = new Transaction
        {
            Amount     = dto.Amount,
            Type       = transactionType,
            AccountId  = dto.AccountId,
            CategoryId = dto.CategoryId,
            Date       = dto.Date.ToUniversalTime(),
            Note       = dto.Note,
            GoalId     = dto.GoalId
        };

        await _transactionRepository.AddAsync(transaction, ct);
        await _transactionRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Transaction created: {Type} {Amount} for user {UserId}",
            transaction.Type, transaction.Amount, userId);

        // Rereading with Include for mapping
        var created = await _transactionRepository.GetByIdAsync(
            transaction.Id, userId, ct);

        return Result<TransactionResponseDto>.Ok(MapToDto(created!));
    }

    public async Task<Result<TransactionResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateTransactionDto dto, CancellationToken ct = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId, ct);
        if (transaction is null)
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Transaction not found.");

        // You cannot edit a transfer directly —
        // you must delete it and create a new one
        if (transaction.Type == TransactionType.Transfer)
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.InvalidOperation,
                "Transfer transactions cannot be edited. Delete and recreate instead.");

        if (!await _categoryRepository.ExistsForUserAsync(dto.CategoryId, userId, ct))
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Category not found.");

        transaction.Amount     = dto.Amount;
        transaction.CategoryId = dto.CategoryId;
        transaction.Date       = dto.Date.ToUniversalTime();
        transaction.Note       = dto.Note;

        await _transactionRepository.UpdateAsync(transaction, ct);
        await _transactionRepository.SaveChangesAsync(ct);

        return Result<TransactionResponseDto>.Ok(MapToDto(transaction));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id, userId, ct);
        if (transaction is null)
            return Result.Fail(ErrorCodes.NotFound, "Transaction not found.");

        // If this is half of Transfer, delete both parts
        if (transaction.Type == TransactionType.Transfer &&
            transaction.TransferPairId.HasValue)
        {
            var pair = await _transactionRepository
                .GetByIdAsync(transaction.TransferPairId.Value, userId, ct);

            if (pair is not null)
                await _transactionRepository.DeleteAsync(pair, ct);
        }

        await _transactionRepository.DeleteAsync(transaction, ct);
        await _transactionRepository.SaveChangesAsync(ct);

        return Result.Ok();
    }

    public async Task<Result<IEnumerable<TransactionExportDto>>> GetForExportAsync(
    Guid userId,
    DateTime dateFrom,
    DateTime dateTo,
    CancellationToken ct = default)
    {
        var transactions = await _transactionRepository.GetAllAsync(
            userId,
            accountId:  null,
            categoryId: null,
            dateFrom:   dateFrom,
            dateTo:     dateTo,
            type:       null,
            page:       1,
            pageSize:   10_000,  // maximum for export
            ct:         ct);

        var dtos = transactions.Select(t => new TransactionExportDto
        {
            Date         = t.Date.ToString("yyyy-MM-dd HH:mm"),
            Type         = t.Type.ToString(),
            Amount       = t.Amount,
            AccountName  = t.Account.Name,
            CategoryName = t.Category.Name,
            Note         = t.Note ?? string.Empty
        });

        return Result<IEnumerable<TransactionExportDto>>.Ok(dtos);
    }

    // -------------------------------------------------------
    // Transfer: Creating Two Linked Transactions

    private async Task<Result<TransactionResponseDto>> CreateTransferAsync(
        Guid userId, CreateTransactionDto dto, CancellationToken ct)
    {
        // ToAccountId has already been validated—it is not null for Transfer
        if (!await _accountRepository.ExistsAsync(dto.ToAccountId!.Value, userId, ct))
            return Result<TransactionResponseDto>.Fail(
                ErrorCodes.NotFound, "Destination account not found.");

        var transferId = Guid.NewGuid();

        // Withdrawal from the original account
        var expense = new Transaction
        {
            Id             = transferId,
            Amount         = dto.Amount,
            Type           = TransactionType.Transfer,
            AccountId      = dto.AccountId,
            CategoryId     = dto.CategoryId,
            Date           = dto.Date.ToUniversalTime(),
            Note           = dto.Note,
        };

        // Deposit to the designated account
        var income = new Transaction
        {
            Amount         = dto.Amount,
            Type           = TransactionType.Transfer,
            AccountId      = dto.ToAccountId.Value,
            CategoryId     = dto.CategoryId,
            Date           = dto.Date.ToUniversalTime(),
            Note           = dto.Note,
            TransferPairId = transferId   // ← Link to the first transaction
        };

        // Let's tie it back
        expense.TransferPairId = income.Id;

        await _transactionRepository.AddRangeAsync([expense, income], ct);
        await _transactionRepository.SaveChangesAsync(ct);

        var created = await _transactionRepository
            .GetByIdAsync(expense.Id, userId, ct);

        return Result<TransactionResponseDto>.Ok(MapToDto(created!));
    }

    // -------------------------------------------------------

    private static TransactionResponseDto MapToDto(Transaction t) => new()
    {
        Id             = t.Id,
        Amount         = t.Amount,
        Type           = t.Type.ToString(),
        AccountName    = t.Account.Name,
        CategoryName   = t.Category.Name,
        CategoryIcon   = t.Category.Icon,
        Date           = t.Date,
        Note           = t.Note,
        TransferPairId = t.TransferPairId,
        GoalId         = t.GoalId,
        CreatedAt      = t.CreatedAt
    };
}