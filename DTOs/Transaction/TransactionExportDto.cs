using CsvHelper.Configuration.Attributes;

namespace FinanceTracker.DTOs.Transaction;

/// <summary>
/// A DTO specifically designed for CSV export.
/// The [Name] attribute specifies the column header in the file.
/// </summary>
public class TransactionExportDto
{
    [Name("Date")]
    public string Date { get; set; } = string.Empty;

    [Name("Type")]
    public string Type { get; set; } = string.Empty;

    [Name("Amount")]
    public decimal Amount { get; set; }

    [Name("Account")]
    public string AccountName { get; set; } = string.Empty;

    [Name("Category")]
    public string CategoryName { get; set; } = string.Empty;

    [Name("Note")]
    public string Note { get; set; } = string.Empty;
}