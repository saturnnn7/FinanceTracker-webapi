namespace FinanceTracker.DTOs.Category;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name  { get; set; } = string.Empty;
    public string Icon  { get; set; } = "📦";
    public string Color { get; set; } = "#6C757D";
    public bool IsSystem { get; set; }
}