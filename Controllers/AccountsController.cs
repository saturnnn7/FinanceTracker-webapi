using FinanceTracker.DTOs.Account;
using FinanceTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/accounts")]
[Authorize]
public class AccountsController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>Get all accounts with current balance</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AccountResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _accountService.GetAllAsync(GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Get account by id</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),             StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _accountService.GetByIdAsync(id, GetUserId(), ct);
        return FromResult(result);
    }

    /// <summary>Create a new account</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AccountResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>),             StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto, CancellationToken ct)
    {
        var result = await _accountService.CreateAsync(GetUserId(), dto, ct);

        if (result.IsFailure)
            return FromResult(result);

        return CreatedResponse(nameof(GetById), new { id = result.Value!.Id }, result.Value!);
    }

    /// <summary>Update account name, color or archive status</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AccountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),             StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody]  UpdateAccountDto dto,
        CancellationToken ct)
    {
        var result = await _accountService.UpdateAsync(id, GetUserId(), dto, ct);
        return FromResult(result);
    }

    /// <summary>Delete account and all its transactions</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _accountService.DeleteAsync(id, GetUserId(), ct);

        if (result.IsFailure)
            return FromResult(result);

        return NoContent();
    }
}