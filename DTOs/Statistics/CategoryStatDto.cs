namespace FinanceTracker.DTOs.Statistics;

/// <summary>Expenses in a single category for the period.</summary>
public class CategoryStatDto
{
    public Guid    CategoryId   { get; set; }
    public string  CategoryName { get; set; } = string.Empty;
    public string  CategoryIcon { get; set; } = string.Empty;
    public string  Color        { get; set; } = string.Empty;
    public decimal Amount       { get; set; }
    public decimal Percent      { get; set; }  // of the total amount for the period
}