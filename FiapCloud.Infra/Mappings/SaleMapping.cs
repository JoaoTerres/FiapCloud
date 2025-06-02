using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class SaleMapping : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
               .HasColumnName("SaleId")
               .ValueGeneratedNever();

        builder.Property(s => s.GameId)
               .IsRequired();

        builder.Property(s => s.Quantity)
               .IsRequired();

        builder.Property(s => s.UnitPrice)
               .IsRequired()
               .HasColumnType("numeric(18,2)");

        builder.Ignore(s => s.TotalPrice);

        builder.Property(s => s.SaleDate)
               .IsRequired()
               .HasColumnType("timestamp with time zone"); 

    }
}