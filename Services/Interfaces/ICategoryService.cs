using FinanceTracker.Common;
using FinanceTracker.DTOs.Category;

namespace FinanceTracker.Services.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponseDto>>> GetAllAsync(Guid userId, CancellationToken ct = default);

    Task<Result<CategoryResponseDto>> CreateAsync(Guid userId, CreateCategoryDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}