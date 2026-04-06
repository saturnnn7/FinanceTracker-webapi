using FinanceTracker.Models.Interfaces;

namespace FinanceTracker.Models;

/// <summary>
/// 
/// </summary>
public class Goal : IAuditableEntity
{
    public Guid     Id              { get; set; } = Guid.NewGuid();
    public string   Title           { get; set; } = string.Empty;
    public string?  Description     { get; set; }
    public decimal  TargetAmount    { get; set; }
    public DateTime TargetDate      { get; set; }
    public bool     IsCompleted     { get; set; } = false;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }

    // FK
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}