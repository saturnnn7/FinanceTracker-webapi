using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Note)
            .HasMaxLength(500);

        builder.Property(t => t.Type)
            .HasConversion<string>();

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.RecurringTransaction)
            .WithMany(r => r.Transactions)
            .HasForeignKey(t => t.RecurringTransactionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(t => t.Goal)
            .WithMany(g => g.Transactions)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
        
        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.Date);

        builder.HasIndex(t => new { t.AccountId, t.Date });
    }
}