using FinanceTracker.Models.Interfaces;

namespace FinanceTracker.Models;

/// <summary>
/// Budget — the spending limit for a category over a given period (month).
/// Progress is calculated dynamically based on transactions for the period;
/// it is not stored as a field to prevent desynchronization.
/// </summary>
public class Budget : IAuditableEntity
{
    public Guid     Id          { get; set; } = Guid.NewGuid();
    public decimal  LimitAmount { get; set; }

    /// <summary>Budget month (1–12).</summary>
    public int      Month       { get; set; }

    /// <summary>Budget year.</summary>
    public int      Year        { get; set; }

    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }

    // FK
    public Guid UserId      { get; set; }
    public Guid CategoryId  { get; set; }

    // Navigation properties
    public User     User        { get; set; } = null!;
    public Category Category    { get; set; } = null!;
}