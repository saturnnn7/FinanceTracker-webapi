namespace FinanceTracker.DTOs.Account;

public class AccountResponseDto
{
    public Guid Id                  { get; set; }
    public string Name              { get; set; } = string.Empty;
    public string Type              { get; set; } = string.Empty;
    public decimal InitialBalance   { get; set; }
    public decimal CurrentBalance   { get; set; } // is calculated by the service
    public string Currency          { get; set; } = string.Empty;
    public string Color             { get; set; } = string.Empty;
    public bool IsArchived          { get; set; }
    public DateTime CreatedAt       { get; set; }
}