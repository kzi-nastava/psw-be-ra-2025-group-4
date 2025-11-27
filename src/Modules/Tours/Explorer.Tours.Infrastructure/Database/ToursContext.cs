using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

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

        public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tours");

            modelBuilder.Entity<Tour>()
                .HasMany(t => t.Points)
                .WithOne(p => p.Tour)
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
