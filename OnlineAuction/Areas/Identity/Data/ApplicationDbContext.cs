
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineAuction.Areas.Identity.Data;
using OnlineAuction.Models;

namespace OnlineAuction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        // New DbSet for Bid
        public DbSet<Bid> Bids { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relationships configured as per previous step
            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Products)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.Bids)
                .WithOne(b => b.ApplicationUser)
                .HasForeignKey(b => b.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(p => p.Bids)
                .WithOne(b => b.Product)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
                .Property(p => p.MinimumBid)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Bid>()
                .Property(b => b.BidAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}

