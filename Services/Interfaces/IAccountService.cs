using FinanceTracker.Common;
using FinanceTracker.DTOs.Account;

namespace FinanceTracker.Services.Interfaces;

public interface IAccountService
{
    Task<Result<IEnumerable<AccountResponseDto>>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<Result<AccountResponseDto>> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<Result<AccountResponseDto>> CreateAsync(Guid userId, CreateAccountDto dto, CancellationToken ct = default);
    Task<Result<AccountResponseDto>> UpdateAsync(Guid id, Guid userId, UpdateAccountDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}