using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class LibraryMapping : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
               .HasColumnName("Id")
               .ValueGeneratedNever();

        builder.Property(l => l.UserId)
               .HasColumnName("UserId")
               .IsRequired();

        builder.Metadata
               .FindNavigation(nameof(Library.Sales))
               ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        // Navigation: Sales
        builder.HasMany(l => l.Sales)
               .WithOne()
               .HasForeignKey("LibraryId")
               .OnDelete(DeleteBehavior.Cascade);
    }
}
