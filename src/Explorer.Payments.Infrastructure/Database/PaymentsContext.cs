using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("payments");

            modelBuilder.Entity<ShoppingCart>(builder =>
            {
                builder.ToTable("ShoppingCarts");

                builder.HasKey(c => c.Id);
                builder.Property(c => c.TouristId).IsRequired();
                builder.Property(c => c.TotalPrice).IsRequired();

                builder.OwnsMany(c => c.Items, owned =>
                {
                    owned.ToTable("OrderItem"); // ili "OrderItems"
                    owned.WithOwner().HasForeignKey("ShoppingCartId");

                    owned.Property<int>("Id");
                    owned.HasKey("Id");

                    owned.Property(o => o.TourId).IsRequired();
                    owned.Property(o => o.TourName).IsRequired();
                    owned.Property(o => o.Price).IsRequired();
                });
            });

            modelBuilder.Entity<TourPurchaseToken>(builder =>
            {
                builder.ToTable("TourPurchaseTokens");

                builder.HasKey(t => t.Id);

                builder.Property(t => t.TouristId).IsRequired();
                builder.Property(t => t.TourId).IsRequired();
                builder.Property(t => t.PurchasedAt).IsRequired();
                builder.HasIndex(t => new { t.TouristId, t.TourId }).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
