using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class GameMapping : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        // Table
        builder.ToTable("Games");

        // Key
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id)
               .HasColumnName("Id")
               .ValueGeneratedNever();

        // Properties
        builder.Property(g => g.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(g => g.Description)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(g => g.Price)
               .HasColumnType("numeric(18,2)")
               .IsRequired();

        builder.Metadata
               .FindNavigation(nameof(Game.GamePromotions))
               ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata
               .FindNavigation(nameof(Game.Sales))
               ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(g => g.GamePromotions)
               .WithOne()
               .HasForeignKey(p => p.GameId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Sales)
               .WithOne()
               .HasForeignKey(s => s.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}