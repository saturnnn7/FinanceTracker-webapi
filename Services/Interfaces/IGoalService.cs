using FinanceTracker.Common;
using FinanceTracker.DTOs.Goal;

namespace FinanceTracker.Services.Interfaces;

public interface IGoalService
{
    Task<Result<IEnumerable<GoalResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default);

    Task<Result<GoalResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default);

    Task<Result<GoalResponseDto>> CreateAsync(
        Guid userId, CreateGoalDto dto, CancellationToken ct = default);

    Task<Result<GoalResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateGoalDto dto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}