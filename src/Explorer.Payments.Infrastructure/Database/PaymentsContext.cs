using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<AffiliateCode> AffiliateCodes { get; set; }


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
                    owned.ToTable("OrderItem");
                    owned.WithOwner().HasForeignKey("ShoppingCartId");
                    owned.Property<int>("Id");
                    owned.HasKey("Id");
                    owned.Property<int>("TourId").IsRequired();
                    owned.Property<int?>("BundleId").IsRequired(false);
                    owned.Property<string>("TourName").IsRequired();
                    owned.Property<decimal>("Price").IsRequired();
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

            modelBuilder.Entity<Wallet>(builder =>
            {
                builder.ToTable("Wallets");
                builder.HasKey(w => w.Id);
                builder.Property(w => w.TouristId).IsRequired();
                builder.Property(w => w.Balance).IsRequired().HasColumnType("decimal(18,2)");
                builder.HasIndex(w => w.TouristId).IsUnique();
            });

            modelBuilder.Entity<PaymentRecord>(builder =>
            {
                builder.ToTable("PaymentRecords");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.TouristId).IsRequired();
                builder.Property(p => p.TourId).IsRequired(false);
                builder.Property(p => p.BundleId).IsRequired(false);
                builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(p => p.PurchaseTime).IsRequired();
            });

            modelBuilder.Entity<Coupon>(builder =>
            {
                builder.ToTable("Coupons");
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Code).IsRequired().HasMaxLength(8);
                builder.Property(c => c.DiscountPercentage).IsRequired();
                builder.Property(c => c.AuthorId).IsRequired();
                builder.Property(c => c.TourId).IsRequired(false); 
                builder.Property(c => c.ExpirationDate).IsRequired(false); 
                builder.Property(c => c.IsUsed).IsRequired();
                builder.Property(c => c.UsedByTouristId).IsRequired(false); 
                builder.Property(c => c.UsedAt).IsRequired(false); 

                builder.HasIndex(c => c.Code).IsUnique();

                builder.HasIndex(c => c.AuthorId);
            });

            modelBuilder.Entity<AffiliateCode>(builder =>
            {
                builder.ToTable("AffiliateCodes");
                builder.HasKey(a => a.Id);

                builder.Property(a => a.Code).IsRequired().HasMaxLength(16);
                builder.Property(a => a.AuthorId).IsRequired();
                builder.Property(a => a.TourId).IsRequired(false);
                builder.Property(a => a.CreatedAt).IsRequired();
                builder.Property(a => a.Active).IsRequired();

                builder.HasIndex(a => a.Code).IsUnique();
                builder.HasIndex(a => a.AuthorId);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}