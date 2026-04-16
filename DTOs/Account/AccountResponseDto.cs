namespace FinanceTracker.DTOs.Account;

public class AccountResponseDto
{
    public Guid Id                  { get; set; }
    public string Name              { get; set; } = string.Empty;
    public string Type              { get; set; } = string.Empty;
    public decimal InitialBalance   { get; set; }
    public decimal CurrentBalance   { get; set; } // is calculated by the service
    public string Currency          { get; set; } = string.Empty;

    /// <summary>
    /// Balance converted to the user's primary currency.
    /// Equals CurrentBalance if the account currency matches the base currency.
    /// </summary>
    public decimal  CurrentBalanceConverted { get; set; }

    /// <summary>The user's primary currency—the currency to which it was converted.</summary>
    public string   BaseCurrency            { get; set; } = string.Empty;
    
    public string Color             { get; set; } = string.Empty;
    public bool IsArchived          { get; set; }
    public DateTime CreatedAt       { get; set; }
}