using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class PromotionMapping : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever();

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.InitialDate)
                   .IsRequired()
                   .HasColumnType("timestamp with time zone");

            builder.Property(p => p.FinalDate)
                   .IsRequired()
                   .HasColumnType("timestamp with time zone"); 

            builder.Property(p => p.Percentage)
                   .HasColumnType("numeric(5,2)")
                   .IsRequired();

            builder.Property(p => p.Enable)
                   .IsRequired();

            builder.Metadata
                   .FindNavigation(nameof(Promotion.GamePromotions))
                   ?.SetPropertyAccessMode(PropertyAccessMode.Field);
            builder.Metadata
                   .FindNavigation(nameof(Promotion.Sales))
                   ?.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(p => p.GamePromotions)
                   .WithOne()
                   .HasForeignKey(gp => gp.PromotionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Sales)
                   .WithOne()
                   .HasForeignKey("PromotionId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }