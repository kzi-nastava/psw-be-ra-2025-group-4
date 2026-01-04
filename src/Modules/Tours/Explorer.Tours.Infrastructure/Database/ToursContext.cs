using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Explorer.Tours.Infrastructure.Database
{
    public class ToursContext : DbContext
    {
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Facility> Facility { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<TourPreferences> TourPreferences { get; set; }
        public DbSet<TouristEquipment> TouristEquipment { get; set; }
        public DbSet<TourProblem> TourProblems { get; set; }
        public DbSet<HistoricalMonument> HistoricalMonuments { get; set; }
        public DbSet<TourPoint> TourPoints { get; set; }
        public DbSet<TourExecution> TourExecutions { get; set; }
        public DbSet<TourReview> TourReviews { get; set; }

        public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tours");

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Points)
                .WithOne(p => p.Tour)
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Equipment)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "TourEquipment",
                    j => j
                        .HasOne<Equipment>()
                        .WithMany()
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Tour>()
                        .WithMany()
                        .HasForeignKey("TourId")
                        .OnDelete(DeleteBehavior.Cascade)
                );

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Points)
                .WithOne(p => p.Tour)
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tour>()
                .Property(t => t.Tags)
                .HasColumnType("text[]");

            modelBuilder.Entity<Tour>()
                .Property(t => t.TransportDuration)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<TourTransportDuration>>(v, (JsonSerializerOptions)null) ?? new List<TourTransportDuration>()
                );


            modelBuilder.Entity<TourExecution>()
                .HasMany(te => te.CompletedPoints)
                .WithOne()
                .HasForeignKey("TourExecutionId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TourReview>()
                        .Property(tr => tr.Images)
                        .HasColumnType("jsonb")
                        .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                        );


            base.OnModelCreating(modelBuilder);

        }
    }
}
