using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

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

            base.OnModelCreating(modelBuilder);
        }
    }
}
