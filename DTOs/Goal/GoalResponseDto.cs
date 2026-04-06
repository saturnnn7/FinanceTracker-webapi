namespace FinanceTracker.DTOs.Goal;

public class GoalResponseDto
{
    public Guid     Id             { get; set; }
    public string   Title          { get; set; } = string.Empty;
    public string?  Description    { get; set; }
    public decimal  TargetAmount   { get; set; }

    /// <summary>Accumulated — the total amount of transactions linked to the goal.</summary>
    public decimal  SavedAmount    { get; set; }

    /// <summary>Progress (0–100%).</summary>
    public decimal  Progress       => TargetAmount == 0 ? 0
                                      : Math.Round(SavedAmount / TargetAmount * 100, 1);
    public DateTime TargetDate     { get; set; }
    public bool     IsCompleted    { get; set; }
    public DateTime CreatedAt      { get; set; }
}