using FiapCloud.Infra.Identity;
using FiapCLoud.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiapCloud.Infra.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Game> Games { get; set; }
    public DbSet<GamePromotion> GamePromotions { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<DomainUser> DomainUsers { get; set; }
    public DbSet<Wallet> Wallets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
