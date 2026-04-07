using FinanceTracker.Common;
using FinanceTracker.DTOs.Goal;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository        _goalRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GoalService>   _logger;

    public GoalService(
        IGoalRepository goalRepository,
        ITransactionRepository transactionRepository,
        ILogger<GoalService> logger)
    {
        _goalRepository        = goalRepository;
        _transactionRepository = transactionRepository;
        _logger                = logger;
    }

    public async Task<Result<IEnumerable<GoalResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default)
    {
        var goals = await _goalRepository.GetAllByUserAsync(userId, ct);

        var dtos = new List<GoalResponseDto>();
        foreach (var goal in goals)
        {
            var saved = await GetSavedAmountAsync(goal.Id, ct);
            dtos.Add(MapToDto(goal, saved));
        }

        return Result<IEnumerable<GoalResponseDto>>.Ok(dtos);
    }

    public async Task<Result<GoalResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var goal = await _goalRepository.GetByIdAsync(id, userId, ct);
        if (goal is null)
            return Result<GoalResponseDto>.Fail(
                ErrorCodes.NotFound, "Goal not found.");

        var saved = await GetSavedAmountAsync(goal.Id, ct);
        return Result<GoalResponseDto>.Ok(MapToDto(goal, saved));
    }

    public async Task<Result<GoalResponseDto>> CreateAsync(
        Guid userId, CreateGoalDto dto, CancellationToken ct = default)
    {
        var goal = new Goal
        {
            Title        = dto.Title,
            Description  = dto.Description,
            TargetAmount = dto.TargetAmount,
            TargetDate   = dto.TargetDate.ToUniversalTime(),
            UserId       = userId
        };

        await _goalRepository.AddAsync(goal, ct);
        await _goalRepository.SaveChangesAsync(ct);

        return Result<GoalResponseDto>.Ok(MapToDto(goal, 0));
    }

    public async Task<Result<GoalResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateGoalDto dto, CancellationToken ct = default)
    {
        var goal = await _goalRepository.GetByIdAsync(id, userId, ct);
        if (goal is null)
            return Result<GoalResponseDto>.Fail(
                ErrorCodes.NotFound, "Goal not found.");

        goal.Title        = dto.Title;
        goal.Description  = dto.Description;
        goal.TargetAmount = dto.TargetAmount;
        goal.TargetDate   = dto.TargetDate.ToUniversalTime();

        // Automatically mark as completed if the total is greater than or equal to the target
        var saved = await GetSavedAmountAsync(goal.Id, ct);
        if (saved >= goal.TargetAmount)
            goal.IsCompleted = true;

        await _goalRepository.UpdateAsync(goal, ct);
        await _goalRepository.SaveChangesAsync(ct);

        return Result<GoalResponseDto>.Ok(MapToDto(goal, saved));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var goal = await _goalRepository.GetByIdAsync(id, userId, ct);
        if (goal is null)
            return Result.Fail(ErrorCodes.NotFound, "Goal not found.");

        await _goalRepository.DeleteAsync(goal, ct);
        await _goalRepository.SaveChangesAsync(ct);

        return Result.Ok();
    }

    // -------------------------------------------------------

    /// <summary>
    /// The sum of all Income transactions associated with the goal.
    /// </summary>
    private async Task<decimal> GetSavedAmountAsync(Guid goalId, CancellationToken ct)
        => await _transactionRepository
            .GetBalanceByAccountAsync(goalId, ct);

    // -------------------------------------------------------

    private static GoalResponseDto MapToDto(Goal goal, decimal saved) => new()
    {
        Id           = goal.Id,
        Title        = goal.Title,
        Description  = goal.Description,
        TargetAmount = goal.TargetAmount,
        SavedAmount  = saved,
        TargetDate   = goal.TargetDate,
        IsCompleted  = goal.IsCompleted,
        CreatedAt    = goal.CreatedAt
    };
}