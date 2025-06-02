using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities;

namespace FiapCloud.Infra.Mappings;

public class UserMapping : IEntityTypeConfiguration<DomainUser>
{
       public void Configure(EntityTypeBuilder<DomainUser> builder)
       {
              builder.ToTable("DomainUsers");

              builder.HasKey(u => u.Id);
              builder.Property(u => u.Id)
                     .HasColumnName("Id")
                     .ValueGeneratedNever();

              builder.Property(u => u.Name)
                     .IsRequired()
                     .HasMaxLength(200);

              builder.Property(u => u.ApplicationUserId)
                     .IsRequired(); 

              builder.Property(u => u.IsActive)
                     .IsRequired();

              builder.Metadata
                     .FindNavigation(nameof(DomainUser.Library))
                     ?.SetPropertyAccessMode(PropertyAccessMode.Field);
              builder.Metadata
                     .FindNavigation(nameof(DomainUser.Wallet))
                     ?.SetPropertyAccessMode(PropertyAccessMode.Field);

              
              builder.HasOne(u => u.Wallet)
                     .WithOne(w => w.User)
                     .HasForeignKey<Wallet>(w => w.UserId)
                     .OnDelete(DeleteBehavior.Cascade);

              
              builder.HasOne(u => u.Library)
                     .WithOne(l => l.User) 
                     .HasForeignKey<Library>(l => l.UserId) 
                     .OnDelete(DeleteBehavior.Cascade);
       }
}