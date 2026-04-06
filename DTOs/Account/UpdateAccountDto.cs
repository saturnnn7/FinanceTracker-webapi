namespace FinanceTracker.DTOs.Account;

public class UpdateAccountDto
{
    public string Name  { get; set; } = string.Empty;
    public string Color { get; set; } = "#6C757D";

    /// <summary>
    /// Archive account — hide from the main list.
    /// Transactions are saved.
    /// </summary>
    public bool IsArchived { get; set; } = false;
}