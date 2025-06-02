using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FiapCLoud.Domain.Entities; 

namespace FiapCloud.Infra.Mappings;

 public class GamePromotionMapping : IEntityTypeConfiguration<GamePromotion>
    {
        public void Configure(EntityTypeBuilder<GamePromotion> builder)
        {

            builder.ToTable("GamePromotions");

            builder.HasKey(gp => gp.Id);
            builder.Property(gp => gp.Id)
                   .HasColumnName("Id")
                   .ValueGeneratedNever();

            builder.Property(gp => gp.GameId)
                   .IsRequired();

            builder.Property(gp => gp.PromotionId)
                   .IsRequired();

            builder.HasOne(gp => gp.Game)
                   .WithMany(g => g.GamePromotions)
                   .HasForeignKey(gp => gp.GameId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Promotion>()
                   .WithMany()
                   .HasForeignKey(gp => gp.PromotionId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
