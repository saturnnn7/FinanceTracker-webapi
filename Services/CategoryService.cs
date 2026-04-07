using FinanceTracker.Common;
using FinanceTracker.DTOs.Category;
using FinanceTracker.DTOs.Common;
using FinanceTracker.Models;
using FinanceTracker.Repositories.Interfaces;
using FinanceTracker.Services.Interfaces;

namespace FinanceTracker.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _logger             = logger;
    }

    public async Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync(
        Guid userId, CancellationToken ct = default)
    {
        var categories = await _categoryRepository.GetAllForUserAsync(userId, ct);
        return Result<IEnumerable<CategoryResponseDto>>.Ok(
            categories.Select(MapToDto));
    }

    public async Task<Result<CategoryResponseDto>> CreateAsync(
        Guid userId, CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = new Category
        {
            Name     = dto.Name,
            Icon     = dto.Icon,
            Color    = dto.Color,
            IsSystem = false,
            UserId   = userId
        };

        await _categoryRepository.AddAsync(category, ct);
        await _categoryRepository.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Category created: {Name} for user {UserId}", category.Name, userId);

        return Result<CategoryResponseDto>.Ok(MapToDto(category));
    }

    public async Task<Result> DeleteAsync(
        Guid id, Guid userId, CancellationToken ct = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, ct);

        if (category is null || (category.UserId != userId && !category.IsSystem))
            return Result.Fail(ErrorCodes.NotFound, "Category not found.");

        // System categories cannot be deleted
        if (category.IsSystem)
            return Result.Fail(
                ErrorCodes.InvalidOperation, "System categories cannot be deleted.");

        // Cannot be deleted if there are transactions
        if (await _categoryRepository.HasTransactionsAsync(id, ct))
            return Result.Fail(
                ErrorCodes.InvalidOperation,
                "Cannot delete category with existing transactions.");

        await _categoryRepository.DeleteAsync(category, ct);
        await _categoryRepository.SaveChangesAsync(ct);

        return Result.Ok();
    }

    // -------------------------------------------------------

    private static CategoryResponseDto MapToDto(Category category) => new()
    {
        Id       = category.Id,
        Name     = category.Name,
        Icon     = category.Icon,
        Color    = category.Color,
        IsSystem = category.IsSystem
    };
}