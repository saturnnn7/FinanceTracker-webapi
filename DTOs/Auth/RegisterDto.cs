namespace FinanceTracker.DTOs.Auth;

public class RegisterDto
{
    public string Username  { get; set; } = string.Empty;
    public string Email     { get; set; } = string.Empty;
    public string Password  { get; set; } = string.Empty;

    /// <summary>
    /// ISO 4217: PLN, USD, EUR.
    /// </summary>
    public string BaseCurrency { get; set; } = "PNL";
}