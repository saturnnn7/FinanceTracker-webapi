namespace FinanceTracker.Models.Interfaces;

/// <summary>
/// A marker interface for entities with creation and update dates.
/// AuditInterceptor automatically populates these fields when SaveChanges is called.
/// All models except helper models implement this interface.
/// </summary>
public interface IAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt  { get; set; }
}