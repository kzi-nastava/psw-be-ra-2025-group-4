using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database;

public class EncountersContext : DbContext
{
    public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) {}

    public DbSet<Encounter> Encounters { get; set; } = null!;
    public DbSet<EncounterExecution> EncounterExecutions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("encounters");

        modelBuilder.Entity<Encounter>()
               .Property(e => e.Location)
               .HasColumnType("jsonb")
               .HasConversion(
                   // Convert Location to JSON string for database
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                   {
                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                   }),
                   // Convert JSON string back to Location object
                   v => JsonSerializer.Deserialize<Location>(v, new JsonSerializerOptions
                   {
                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                   })
               );

        modelBuilder.Entity<Encounter>()
            .Property(e => e.TourPointId)
            .IsRequired(false);

        modelBuilder.Entity<EncounterExecution>()
            .ToTable("EncounterExecutions");

        modelBuilder.Entity<HiddenLocationEncounter>()
            .ToTable("HiddenLocationEncounters");

        modelBuilder.Entity<HiddenLocationEncounter>()
            .Property(h => h.PhotoPoint)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Location>(v, (JsonSerializerOptions?)null)!
            );

        modelBuilder.Entity<SocialEncounter>()
            .ToTable("SocialEncounters");

        modelBuilder.Entity<EncounterParticipant>()
            .ToTable("EncounterParticipants")
            .HasKey(p => p.Id);

        modelBuilder.Entity<QuizEncounter>()
            .ToTable("QuizEncounters")
            .HasMany(e => e.Questions)
            .WithOne()
            .HasForeignKey(q => q.QuizEncounterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuizAnswer>()
            .ToTable("QuizAnswers");

        modelBuilder.Entity<QuizQuestion>()
            .ToTable("QuizQuestions")
            .HasMany(q => q.Answers)
            .WithOne()
            .HasForeignKey(a => a.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
