using FinanceTracker.Common;
using FinanceTracker.Data;
using FinanceTracker.DTOs.Goal;
using FinanceTracker.Models;
using FinanceTracker.Models.Enums;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository        _goalRepository;
    private readonly AppDbContext           _context;
    private readonly ILogger<GoalService>   _logger;

    public GoalService(
        IGoalRepository goalRepository,
        AppDbContext context,
        ILogger<GoalService> logger)
    {
        _goalRepository = goalRepository;
        _context        = context;
        _logger         = logger;
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

        _logger.LogInformation(
            "Goal created: {Title} for user {UserId}", goal.Title, userId);

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

        var saved = await GetSavedAmountAsync(goal.Id, ct);

        // Automatically mark as completed if the total is greater than or equal to the target
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
    /// Calculate the total amount accumulated—the sum of all Income transactions
    /// where GoalId == goalId. SQLite does not support SumAsync(decimal)
    /// so we load the data into memory using ToListAsync.
    /// </summary>
    private async Task<decimal> GetSavedAmountAsync(Guid goalId, CancellationToken ct)
    {
        var amounts = await _context.Transactions
            .Where(t => t.GoalId == goalId && t.Type == TransactionType.Income)
            .Select(t => t.Amount)
            .ToListAsync(ct);

        return amounts.Sum();
    }

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