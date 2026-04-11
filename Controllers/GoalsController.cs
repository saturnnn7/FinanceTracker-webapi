using FinanceTracker.DTOs.Goal;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/goals")]
[Authorize]
public class GoalsController : BaseController
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    /// <summary>Get all savings goals with progress</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GoalResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _goalService.GetAllAsync(GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Get goal by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GoalResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _goalService.GetByIdAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Create a new savings goal</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GoalResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGoalDto dto, CancellationToken ct)
    {
        var result = await _goalService.CreateAsync(GetUserId(), dto, ct);

        if (result.IsFailure)
            return FromResult(result);

        return CreatedResponse(nameof(GetById), new { id = result.Value!.Id }, result.Value!);
    }

    /// <summary>Update goal details</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GoalResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),          StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody]  UpdateGoalDto dto,
        CancellationToken ct)
    {
        var result = await _goalService.UpdateAsync(id, GetUserId(), dto, ct);
        return FromResult(result);
    }

    /// <summary>Delete goal</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _goalService.DeleteAsync(id, GetUserId(), ct);

        if (result.IsFailure)
            return FromResult(result);

        return NoContent();
    }
}