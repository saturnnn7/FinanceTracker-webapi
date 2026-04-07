using FinanceTracker.Common;
using FinanceTracker.DTOs.Budget;

namespace FinanceTracker.Services.Interfaces;

public interface IBudgetService
{
    Task<Result<IEnumerable<BudgetResponseDto>>> GetAllAsync(
        Guid userId, int month, int year, CancellationToken ct = default);

    Task<Result<BudgetResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default);

    Task<Result<BudgetResponseDto>> CreateAsync(
        Guid userId, CreateBudgetDto dto, CancellationToken ct = default);

    Task<Result<BudgetResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateBudgetDto dto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}