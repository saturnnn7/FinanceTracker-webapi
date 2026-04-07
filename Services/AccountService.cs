using FinanceTracker.Common;
using FinanceTracker.DTOs.Account;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository     _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        ILogger<AccountService> logger)
    {
        _accountRepository     = accountRepository;
        _transactionRepository = transactionRepository;
        _logger                = logger;
    }

    public async Task<Result<IEnumerable<AccountResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default)
    {
        var accounts = await _accountRepository.GetAllByUserAsync(userId, ct);

        // For each account, we calculate the current balance
        var dtos = new List<AccountResponseDto>();
        foreach (var account in accounts)
        {
            var balance = await _transactionRepository
                .GetBalanceByAccountAsync(account.Id, ct);

            dtos.Add(MapToDto(account, account.InitialBalance + balance));
        }

        return Result<IEnumerable<AccountResponseDto>>.Ok(dtos);
    }

    public async Task<Result<AccountResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, userId, ct);
        if (account is null)
            return Result<AccountResponseDto>.Fail(
                ErrorCodes.NotFound, "Account not found.");

        var balance = await _transactionRepository
            .GetBalanceByAccountAsync(account.Id, ct);

        return Result<AccountResponseDto>.Ok(
            MapToDto(account, account.InitialBalance + balance));
    }

    public async Task<Result<AccountResponseDto>> CreateAsync(
        Guid userId, CreateAccountDto dto, CancellationToken ct = default)
    {
        // Parse an enum from a string — the validator has already verified that the string is valid
        var accountType = Enum.Parse<AccountType>(dto.Type, true);

        var account = new Account
        {
            Name           = dto.Name,
            Type           = accountType,
            InitialBalance = dto.InitialBalance,
            Currency       = dto.Currency.ToUpper(),
            Color          = dto.Color,
            UserId         = userId
        };

        await _accountRepository.AddAsync(account, ct);
        await _accountRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Account created: {Name} for user {UserId}", account.Name, userId);

        // New account — balance equals opening balance
        return Result<AccountResponseDto>.Ok(
            MapToDto(account, account.InitialBalance));
    }

    public async Task<Result<AccountResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateAccountDto dto, CancellationToken ct = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, userId, ct);
        if (account is null)
            return Result<AccountResponseDto>.Fail(
                ErrorCodes.NotFound, "Account not found.");

        account.Name       = dto.Name;
        account.Color      = dto.Color;
        account.IsArchived = dto.IsArchived;

        await _accountRepository.UpdateAsync(account, ct);
        await _accountRepository.SaveChangesAsync(ct);

        var balance = await _transactionRepository
            .GetBalanceByAccountAsync(account.Id, ct);

        return Result<AccountResponseDto>.Ok(
            MapToDto(account, account.InitialBalance + balance));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, userId, ct);
        if (account is null)
            return Result.Fail(ErrorCodes.NotFound, "Account not found.");

        await _accountRepository.DeleteAsync(account, ct);
        await _accountRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Account deleted: {Id} for user {UserId}", id, userId);

        return Result.Ok();
    }

    // -------------------------------------------------------

    private static AccountResponseDto MapToDto(Account account, decimal currentBalance) => new()
    {
        Id             = account.Id,
        Name           = account.Name,
        Type           = account.Type.ToString(),
        InitialBalance = account.InitialBalance,
        CurrentBalance = currentBalance,
        Currency       = account.Currency,
        Color          = account.Color,
        IsArchived     = account.IsArchived,
        CreatedAt      = account.CreatedAt
    };
}