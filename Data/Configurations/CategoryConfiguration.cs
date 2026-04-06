using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Icon)
            .HasMaxLength(10);

        builder.Property(c => c.Color)
            .HasMaxLength(7); // #RRGGBB

        builder.HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false); // null FK

        builder.HasIndex(c => c.UserId);

        builder.HasData(
            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), 
                Name = "Food and Restaurants", 
                Icon = "🍔", 
                Color = "#FF6B6B", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },
            
            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), 
                Name = "Transport", 
                Icon = "🚗", 
                Color = "#4ECDC4", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), 
                Name = "Housing", 
                Icon = "🏠", 
                Color = "#45B7D1", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), 
                Name = "Health", 
                Icon = "💊", 
                Color = "#96CEB4", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), 
                Name = "Entertainment", 
                Icon = "🎮", 
                Color = "#FFEAA7", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), 
                Name = "Clothing",
                Icon = "👕", 
                Color = "#DDA0DD", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), 
                Name = "Salary", 
                Icon = "💰", 
                Color = "#98D8C8", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), 
                Name = "Freelance", 
                Icon = "💻", 
                Color = "#F7DC6F", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000009"), 
                Name = "Subscriptions", 
                Icon = "📱", 
                Color = "#BB8FCE", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) },

            new Category { Id = Guid.Parse("10000000-0000-0000-0000-000000000010"), 
                Name = "Other", 
                Icon = "📦", 
                Color = "#6C757D", 
                IsSystem = true, 
                CreatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc), 
                UpdatedAt = DateTime.SpecifyKind(new DateTime(2026,4,6), DateTimeKind.Utc) }
        );
    }
}