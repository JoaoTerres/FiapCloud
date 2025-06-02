using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class WalletMapping : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id)
               .HasColumnName("Id")
               .ValueGeneratedNever();

        builder.Property(w => w.UserId)
               .IsRequired();

        builder.Property<decimal>("_balance")
               .HasColumnName("Balance")
               .HasColumnType("numeric(18,2)")
               .IsRequired();

        builder.Metadata
               .FindNavigation(nameof(Wallet.User))
               ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(w => w.User)
               .WithOne(u => u.Wallet)
               .HasForeignKey<Wallet>(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
