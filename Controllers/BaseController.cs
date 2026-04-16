using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

using FinanceTracker.Common;
using FinanceTracker.DTOs.Common;
using FinanceTracker.DTOs.Transaction;

namespace FinanceTracker.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Retrieves the user's BaseCurrency from the JWT claims.
    /// The claim is added during login in the AuthService.
    /// </summary>
    protected string GetBaseCurrency() =>
        User.FindFirstValue("currency") ?? "PLN";

    /// <summary>
    /// UserId from JWT token
    /// </summary>
    protected Guid GetUserId()
    {
        var claims = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claims is null ? Guid.Empty : Guid.Parse(claims);
    }

    // --------------------------------------------------
    // Helpers for successful responses
    /// <summary>
    /// -200 OK- with data
    /// </summary>
    protected IActionResult OkResponse<T>(T data) =>
        Ok(ApiResponse<T>.Ok(data));

    /// <summary>
    /// -201 Created- 
    /// </summary>
    protected IActionResult CreatedResponse<T>(string actionName, object routeValues, T data) =>
        CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data));

    /// <summary>
    /// -204 No Content- For Delete
    /// </summary>
    protected new IActionResult NoContent() =>
        base.NoContent();

    // --------------------------------------------------
    // Helpers for Errors
    /// <summary>
    /// -404 NotFound-
    /// </summary>
    protected IActionResult NotFoundResponse(string message = "Resource not found.") =>
        NotFound(ApiResponse<object>.Fail(ErrorCodes.NotFound, message));

    /// <summary>
    /// -409 Conflict-
    /// </summary>
    protected IActionResult ConflictResponse(string message) =>
        Conflict(ApiResponse<object>.Fail(ErrorCodes.Conflict, message));

    /// <summary>
    /// -401 Unauthorized-
    /// </summary>
    protected IActionResult UnauthorizedResponse(string message = "Invalid credentials.") =>
        Unauthorized(ApiResponse<object>.Fail(ErrorCodes.Unauthorized, message));

    /// <summary>
    /// -403 Forbidden-
    /// </summary>
    protected IActionResult ForbiddenResponse(string message = "Access denied.") =>
        StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Fail(ErrorCodes.Forbidden, message));

    /// <summary>
    /// --
    /// </summary>
    protected IActionResult InvalidOperationResponse(string message) =>
        UnprocessableEntity(ApiResponse<object>.Fail(ErrorCodes.InvalidOperation, message));


    // --------------------------------------------------
    // Bridge Result -> IActionResult

    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return OkResponse(result.Value!);
        
        return result.ErrorCode switch
        {
            ErrorCodes.NotFound         => NotFoundResponse(result.ErrorMessage!),
            ErrorCodes.Conflict         => ConflictResponse(result.ErrorMessage!),
            ErrorCodes.Unauthorized     => UnauthorizedResponse(result.ErrorMessage!),
            ErrorCodes.Forbidden        => ForbiddenResponse(result.ErrorMessage!),
            ErrorCodes.InvalidOperation => InvalidOperationResponse(result.ErrorMessage!),
            _                           => StatusCode(500, 
                                            ApiResponse<object>.Fail(
                                                ErrorCodes.InternalError, result.ErrorMessage ?? "Unexpected error."))
        };
    }

    protected IActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();
    
        return result.ErrorCode switch
        {
            ErrorCodes.NotFound         => NotFoundResponse(result.ErrorMessage!),
            ErrorCodes.Conflict         => ConflictResponse(result.ErrorMessage!),
            ErrorCodes.Unauthorized     => UnauthorizedResponse(result.ErrorMessage!),
            ErrorCodes.Forbidden        => ForbiddenResponse(result.ErrorMessage!),
            ErrorCodes.InvalidOperation => InvalidOperationResponse(result.ErrorMessage!),
            _                           => StatusCode(500,
                                             ApiResponse<object>.Fail(
                                                 ErrorCodes.InternalError,
                                                 result.ErrorMessage ?? "Unexpected error."))
        };
    }
}