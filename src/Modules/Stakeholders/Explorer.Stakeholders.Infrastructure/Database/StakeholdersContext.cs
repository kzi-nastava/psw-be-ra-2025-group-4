using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<DirectMessage> DirectMessages { get; set; }

    public DbSet<Club> Clubs { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<TouristLocation> TouristLocations { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<ClubMessage> ClubMessages { get; set; }
    public DbSet<Follow> Follows { get; set; }
  
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<UserAchievements> UserAchievements { get; set; } = null!;

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Notification>();

        ConfigureStakeholder(modelBuilder);
        ConfigureDirectMessage(modelBuilder);
        ConfigureFollow(modelBuilder);
        ConfigureUserAchievements(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }

    private static void ConfigureDirectMessage(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DirectMessage>()
            .HasOne(dm => dm.Sender)
            .WithMany()
            .HasForeignKey(dm => dm.SenderId);

        modelBuilder.Entity<DirectMessage>()
            .HasOne(dm => dm.Recipient)
            .WithMany()
            .HasForeignKey(dm => dm.RecipientId);
    }
    private static void ConfigureFollow(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followed)
            .WithMany()
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FollowedId })
            .IsUnique();
    }

    private static void ConfigureUserAchievements(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAchievements>(ua =>
        {
            ua.HasKey(x => x.Id);
            ua.HasIndex(x => x.UserId).IsUnique();

            ua.HasMany(x => x.Achievements)
                .WithOne()
                .HasForeignKey("UserAchievementsId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            ua.Navigation(x => x.Achievements)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }



}