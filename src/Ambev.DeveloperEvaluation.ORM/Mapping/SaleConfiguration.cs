using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
               .HasColumnType("uniqueidentifier")
               .HasDefaultValueSql("NEWID()");

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(30);
        builder.Property(s => s.Date).IsRequired();

        builder.Property(s => s.CustomerId).HasMaxLength(50);
        builder.Property(s => s.CustomerName).HasMaxLength(150);
        builder.Property(s => s.BranchId).HasMaxLength(50);
        builder.Property(s => s.BranchName).HasMaxLength(150);

        builder.Property(s => s.Cancelled).HasColumnType("bit").IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);

        builder.HasMany(s => s.Items)
               .WithOne(i => i.Sale!)
               .HasForeignKey(i => i.SaleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
