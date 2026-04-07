using FinanceTracker.Common;
using FinanceTracker.DTOs.Budget;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository      _budgetRepository;
    private readonly ICategoryRepository    _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(
        IBudgetRepository budgetRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        ILogger<BudgetService> logger)
    {
        _budgetRepository      = budgetRepository;
        _categoryRepository    = categoryRepository;
        _transactionRepository = transactionRepository;
        _logger                = logger;
    }

    public async Task<Result<IEnumerable<BudgetResponseDto>>> GetAllAsync(
        Guid userId, int month, int year, CancellationToken ct = default)
    {
        var budgets = await _budgetRepository.GetAllByUserAsync(userId, month, year, ct);

        var dtos = new List<BudgetResponseDto>();
        foreach (var budget in budgets)
        {
            var spent = await _transactionRepository
                .GetSpentByCategoryAsync(userId, budget.CategoryId, month, year, ct);

            dtos.Add(MapToDto(budget, spent));
        }

        return Result<IEnumerable<BudgetResponseDto>>.Ok(dtos);
    }

    public async Task<Result<BudgetResponseDto>> GetByIdAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(id, userId, ct);
        if (budget is null)
            return Result<BudgetResponseDto>.Fail(
                ErrorCodes.NotFound, "Budget not found.");

        var spent = await _transactionRepository
            .GetSpentByCategoryAsync(
                userId, budget.CategoryId, budget.Month, budget.Year, ct);

        return Result<BudgetResponseDto>.Ok(MapToDto(budget, spent));
    }

    public async Task<Result<BudgetResponseDto>> CreateAsync(
        Guid userId, CreateBudgetDto dto, CancellationToken ct = default)
    {
        // The category must be available to the user
        if (!await _categoryRepository.ExistsForUserAsync(dto.CategoryId, userId, ct))
            return Result<BudgetResponseDto>.Fail(
                ErrorCodes.NotFound, "Category not found.");

        // One budget per category per month
        if (await _budgetRepository.ExistsAsync(
                userId, dto.CategoryId, dto.Month, dto.Year, ct))
            return Result<BudgetResponseDto>.Fail(
                ErrorCodes.Conflict,
                "A budget for this category already exists for the selected month.");

        var budget = new Budget
        {
            UserId      = userId,
            CategoryId  = dto.CategoryId,
            LimitAmount = dto.LimitAmount,
            Month       = dto.Month,
            Year        = dto.Year
        };

        await _budgetRepository.AddAsync(budget, ct);
        await _budgetRepository.SaveChangesAsync(ct);

        // Reloading with Include Category
        var created = await _budgetRepository.GetByIdAsync(budget.Id, userId, ct);
        var spent   = await _transactionRepository
            .GetSpentByCategoryAsync(userId, budget.CategoryId, budget.Month, budget.Year, ct);

        return Result<BudgetResponseDto>.Ok(MapToDto(created!, spent));
    }

    public async Task<Result<BudgetResponseDto>> UpdateAsync(
        Guid id, Guid userId, UpdateBudgetDto dto, CancellationToken ct = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(id, userId, ct);
        if (budget is null)
            return Result<BudgetResponseDto>.Fail(
                ErrorCodes.NotFound, "Budget not found.");

        budget.LimitAmount = dto.LimitAmount;

        await _budgetRepository.UpdateAsync(budget, ct);
        await _budgetRepository.SaveChangesAsync(ct);

        var spent = await _transactionRepository
            .GetSpentByCategoryAsync(
                userId, budget.CategoryId, budget.Month, budget.Year, ct);

        return Result<BudgetResponseDto>.Ok(MapToDto(budget, spent));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var budget = await _budgetRepository.GetByIdAsync(id, userId, ct);
        if (budget is null)
            return Result.Fail(ErrorCodes.NotFound, "Budget not found.");

        await _budgetRepository.DeleteAsync(budget, ct);
        await _budgetRepository.SaveChangesAsync(ct);

        return Result.Ok();
    }

    // -------------------------------------------------------

    private static BudgetResponseDto MapToDto(Budget budget, decimal spent) => new()
    {
        Id           = budget.Id,
        CategoryName = budget.Category.Name,
        CategoryIcon = budget.Category.Icon,
        LimitAmount  = budget.LimitAmount,
        SpentAmount  = spent,
        Month        = budget.Month,
        Year         = budget.Year
    };
}