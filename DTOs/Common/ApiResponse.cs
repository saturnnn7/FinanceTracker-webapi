using FinanceTracker.Common;

namespace FinanceTracker.DTOs.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }

    public T? Data { get; set; }

    public ApiError? Error { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // ---------------------------------
    // Fabric Methods

    public static ApiResponse<T> Ok(T data) => new()
    {
        IsSuccess   = true,
        Data        = data
    };

    public static ApiResponse<T> Fail(string code, string message, Dictionary<string, string[]>? details = null) => new()
    {
        IsSuccess = false,
        Error = new ApiError
        {
            Code    = code,
            Message = message,
            Details = details
        }
    };

    public static ApiResponse<T> FromResult(Result<T> result)
    {
        return result.IsSuccess
            ? Ok(result.Value!)
            : Fail(result.ErrorCode!, result.ErrorMessage!);
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse OkEmpty() => new() { IsSuccess = true };
}