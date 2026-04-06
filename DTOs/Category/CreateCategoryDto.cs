namespace FinanceTracker.DTOs.Category;

public class CreateCategoryDto
{
    public string Name  { get; set; } = string.Empty;
    public string Icon  { get; set; } = "📦";
    public string Color { get; set; } = "#6C757D";
}