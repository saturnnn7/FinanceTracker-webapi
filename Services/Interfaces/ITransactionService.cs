using FinanceTracker.Common;
using FinanceTracker.DTOs.Common;
using FinanceTracker.DTOs.Transaction;

namespace FinanceTracker.Services.Interfaces;

public interface ITransactionService
{
    Task<Result<PagedResponse<TransactionResponseDto>>> GetAllAsync(Guid userId, TransactionFilterDto filter, CancellationToken ct = default);
    Task<Result<TransactionResponseDto>> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    Task<Result<TransactionResponseDto>> CreateAsync(Guid userId, CreateTransactionDto dto, CancellationToken ct = default);
    Task<Result<TransactionResponseDto>> UpdateAsync(Guid id, Guid userId, UpdateTransactionDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}