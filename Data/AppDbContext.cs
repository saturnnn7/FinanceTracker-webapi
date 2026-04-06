using FinanceTracker.Data.Configurations;
using FinanceTracker.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public class AppDbContext : DbContext
{
    // DI
    private readonly AuditInterceptor _auditInterceptor;
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        AuditInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    // -------------------------------------------------------
    // DbSet Tables

    public DbSet<Models.User> Users { get; set; }
    public DbSet<Models.Account> Accounts { get; set; }
    public DbSet<Models.Transaction> Transactions { get; set; }
    public DbSet<Models.Category> Categorys { get; set; }
    public DbSet<Models.Budget> Budgets { get; set; }
    public DbSet<Models.Goal> Goal { get; set; }
    public DbSet<Models.RecurringTransaction> RecurringTransactions { get; set; }

    // -------------------------------------------------------

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add Interceptor
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically applies all configurations from the current build.
        // Finds all classes that implement IEntityTypeConfiguration<T>
        // and applies them—no need to list each one manually.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppContext).Assembly);
    }






}