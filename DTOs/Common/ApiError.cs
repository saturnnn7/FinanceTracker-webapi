namespace FinanceTracker.DTOs.Common;


/// <summary>
/// The structure of the error in the API response.
/// </summary>
public class ApiError
{
    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public Dictionary<string, string[]>? Details { get; set; }
}