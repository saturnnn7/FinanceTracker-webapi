using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.InitialBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(a => a.Color)
            .HasMaxLength(7); // #RRGGBB


        builder.Property(a => a.Type)
            .HasConversion<string>();

        // 1 User -> many Accounts
        builder.HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.UserId);
    }
}