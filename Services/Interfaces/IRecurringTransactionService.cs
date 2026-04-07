using FinanceTracker.Common;
using FinanceTracker.DTOs.RecurringTransaction;

namespace FinanceTracker.Services.Interfaces;

public interface IRecurringTransactionService
{
    Task<Result<IEnumerable<RecurringTransactionResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default);

    Task<Result<RecurringTransactionResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default);

    Task<Result<RecurringTransactionResponseDto>> CreateAsync(
        Guid userId, CreateRecurringTransactionDto dto, CancellationToken ct = default);

    Task<Result<RecurringTransactionResponseDto>> ToggleActiveAsync(
        Guid id, Guid userId, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}