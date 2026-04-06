namespace FinanceTracker.DTOs.Account;

public class CreateAccountDto
{
    public string Name              { get; set; } = string.Empty;
    public string Type              { get; set; } = string.Empty;
    public decimal InitialBalance   { get; set; } = 0;
    public string Currency          { get; set; } = "PLN";
    public string Color             { get; set; } = "#6C757D";
}