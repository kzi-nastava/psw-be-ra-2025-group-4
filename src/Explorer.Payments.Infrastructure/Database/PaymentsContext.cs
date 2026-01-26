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
        public DbSet<GroupTravelRequest> GroupTravelRequests { get; set; }
        public DbSet<CoinsBundle> CoinsBundles { get; set; }
        public DbSet<CoinsBundleSale> CoinsBundleSales { get; set; }
        public DbSet<CoinsBundlePurchase> CoinsBundlePurchases { get; set; }


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
                builder.Property(a => a.AffiliateTouristId).IsRequired();
                builder.Property(a => a.Percent).IsRequired().HasColumnType("decimal(5,2)");
                builder.Property(a => a.ExpiresAt).IsRequired(false);
                builder.Property(a => a.UsageCount).IsRequired().HasDefaultValue(0);
                builder.Property(a => a.DeactivatedAt).IsRequired(false);
                builder.HasIndex(a => a.Code).IsUnique();
                builder.HasIndex(a => a.AuthorId);
                builder.HasIndex(a => a.AffiliateTouristId);
            });


            modelBuilder.Entity<GroupTravelRequest>(builder =>
            {
                builder.ToTable("GroupTravelRequests");
                builder.HasKey(r => r.Id);
                builder.Property(r => r.OrganizerId).IsRequired();
                builder.Property(r => r.TourId).IsRequired();
                builder.Property(r => r.TourName).IsRequired();
                builder.Property(r => r.PricePerPerson).IsRequired().HasColumnType("decimal(18,2)");
                builder.Property(r => r.Status).IsRequired().HasConversion<int>();
                builder.Property(r => r.CreatedAt).IsRequired();
                builder.Property(r => r.CompletedAt).IsRequired(false);

                builder.HasMany(r => r.Participants)
                    .WithOne()
                    .HasForeignKey("GroupTravelRequestId")
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(r => r.OrganizerId);
                builder.HasIndex(r => r.TourId);
            });

            modelBuilder.Entity<GroupTravelParticipant>(builder =>
            {
                builder.ToTable("GroupTravelParticipants");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.TouristId).IsRequired();
                builder.Property(p => p.Status).IsRequired().HasConversion<int>();
                builder.Property(p => p.RespondedAt).IsRequired(false);

                builder.HasIndex(p => p.TouristId);
            });

            modelBuilder.Entity<CoinsBundle>().HasKey(b => b.Id);
            modelBuilder.Entity<CoinsBundle>().Property(b => b.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<CoinsBundle>().Property(b => b.Price).HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CoinsBundleSale>().HasKey(s => s.Id);
            modelBuilder.Entity<CoinsBundleSale>().Property(s => s.DiscountPercentage).HasColumnType("decimal(5,2)");

            modelBuilder.Entity<CoinsBundlePurchase>().HasKey(p => p.Id);
            modelBuilder.Entity<CoinsBundlePurchase>().Property(p => p.BundleName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<CoinsBundlePurchase>().Property(p => p.PricePaid).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CoinsBundlePurchase>().Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CoinsBundlePurchase>().Property(p => p.TransactionId).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<CoinsBundle>().HasData(
                new
                {
                    Id = 1L,
                    Name = "Starter Pack",
                    Description = "Perfect for trying out the platform",
                    CoinsAmount = 500,
                    BonusCoins = 0,
                    Price = 5.00m,
                    ImageUrl = "bundle-1.png",
                    DisplayOrder = 1
                },
                new
                {
                    Id = 2L,
                    Name = "Explorer Pack",
                    Description = "Great value for casual travelers",
                    CoinsAmount = 1000,
                    BonusCoins = 100,
                    Price = 10.00m,
                    ImageUrl = "bundle-2.png",
                    DisplayOrder = 2
                },
                new
                {
                    Id = 3L,
                    Name = "Adventurer Pack",
                    Description = "Most popular choice!",
                    CoinsAmount = 2000,
                    BonusCoins = 400,
                    Price = 20.00m,
                    ImageUrl = "bundle-3.png",
                    DisplayOrder = 3
                },
                new
                {
                    Id = 4L,
                    Name = "Pro Pack",
                    Description = "For serious explorers",
                    CoinsAmount = 3500,
                    BonusCoins = 850,
                    Price = 35.00m,
                    ImageUrl = "bundle-4.png",
                    DisplayOrder = 4
                },
                new
                {
                    Id = 5L,
                    Name = "Elite Pack",
                    Description = "Maximum savings!",
                    CoinsAmount = 5000,
                    BonusCoins = 1500,
                    Price = 50.00m,
                    ImageUrl = "bundle-5.png",
                    DisplayOrder = 5
                },
                new
                {
                    Id = 6L,
                    Name = "Ultimate Treasure",
                    Description = "The best deal ever!",
                    CoinsAmount = 10000,
                    BonusCoins = 4000,
                    Price = 100.00m,
                    ImageUrl = "bundle-6.png",
                    DisplayOrder = 6
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}