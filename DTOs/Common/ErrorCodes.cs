namespace FinanceTracker.DTOs.Common;


/// <summary>
/// Error code constants. Used in Result.Fail() and ApiResponse.
/// 
/// </summary>
public static class ErrorCodes
{
    public const string ValidationError    = "VALIDATION_ERROR";
    public const string Unauthorized       = "UNAUTHORIZED";
    public const string Forbidden          = "FORBIDDEN";
    public const string NotFound           = "NOT_FOUND";
    public const string Conflict           = "CONFLICT";
    public const string InternalError      = "INTERNAL_ERROR";
    public const string InsufficientFunds  = "INSUFFICIENT_FUNDS";  // for financial transactions
    public const string InvalidOperation   = "INVALID_OPERATION";   // business rules
    public const string ExternalApiError   = "EXTERNAL_API_ERROR";  // exchange rates
}