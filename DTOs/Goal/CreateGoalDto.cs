namespace FinanceTracker.DTOs.Goal;

public class CreateGoalDto
{
    public string   Title        { get; set; } = string.Empty;
    public string?  Description  { get; set; }
    public decimal  TargetAmount { get; set; }
    public DateTime TargetDate   { get; set; }
}