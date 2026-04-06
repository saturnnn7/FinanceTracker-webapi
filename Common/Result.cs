namespace FinanceTracker.Common;

/// <summary>
/// Returns the result of the operation—either a success with data or an error with a code and a message.
/// Used across all services instead of throwing exceptions for business errors.
/// </summary>
public class Result<T>
{
    // ---------------------------------
    // Properties

    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsFailure => !IsSuccess;

    // ---------------------------------
    // Constructor

    private Result() { }

    // ---------------------------------
    // Fabric Methods

    /// <summary>Achieve success with data.</summary>
    public static Result<T> Ok(T value) => new()
    {
        IsSuccess = true,
        Value = value
    };

    /// <summary>Create a result with an error.</summary>
    public static Result<T> Fail(string errorCode, string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };
}

/// <summary>
/// Result without data — for operations that return nothing (deletion, etc.)
/// </summary>
public class Result
{
    // ---------------------------------
    // Properties

    public bool     IsSuccess       { get; private set; }
    public string?  ErrorCode       { get; private set; }
    public string?  ErrorMessage    { get; private set; }
    public bool     isFailure       => !IsSuccess;

    // ---------------------------------
    // Constructor

    private Result() {  }

    // ---------------------------------
    // Fabric Methods

    public static Result Ok() => new() { IsSuccess = true };

    public static Result Fail(string errorCode, string errorMessage) => new()
    {
        IsSuccess       = false,
        ErrorCode       = errorCode,
        ErrorMessage    = errorMessage
    };
}