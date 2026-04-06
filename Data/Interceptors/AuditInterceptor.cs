using FinanceTracker.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FinanceTracker.Data.Interceptors;

/// <summary>
/// EF Core interceptor. Automatically populates CreatedAt and UpdatedAt
/// before each save to the database.
///
/// How it works:
/// - Added   → set CreatedAt and UpdatedAt
/// - Modified → update only UpdatedAt
///
/// Thanks to this, no service needs to modify these fields manually.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        Audit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            Audit(eventData.Context);
            return base.SavingChangesAsync(eventData, result);
        }

    // -------------------------------------------------------

    private static void Audit(DbContext? context)
    {
        if (context is  null) return;

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;

                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }
    }




}