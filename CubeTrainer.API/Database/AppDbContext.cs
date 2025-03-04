using CubeTrainer.API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Database;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Algorithm> Algorithms { get; set; } = default!;

    public DbSet<AlgorithmRating> AlgorithmRatings { get; set; } = default!;

    public DbSet<AlgorithmStatistic> AlgorithmStatistics { get; set; } = default!;

    public DbSet<Case> Cases { get; set; } = default!;

    public DbSet<TrainingPlan> TrainingPlans { get; set; } = default!;

    public DbSet<TrainingPlanCase> TrainingPlanCases { get; set; } = default!;

    public DbSet<UserCase> UserCases { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Algorithm>()
            .HasOne(static a => a.Case)
            .WithMany()
            .HasForeignKey(static a => a.CaseId);
        builder.Entity<Algorithm>()
            .HasOne(static a => a.Creator)
            .WithMany()
            .HasForeignKey(static a => a.CreatorId);
        builder.Entity<Algorithm>()
            .Property(static a => a.Moves)
            .HasMaxLength(500);
        builder.Entity<Algorithm>()
            .Property(static a => a.CreatedAt)
            .HasColumnType("TIMESTAMP");

        builder.Entity<AlgorithmRating>()
            .HasKey(static r => new { r.UserId, r.AlgorithmId });
        builder.Entity<AlgorithmRating>()
            .HasOne(static r => r.Algorithm)
            .WithMany()
            .HasForeignKey(static r => r.AlgorithmId);
        builder.Entity<AlgorithmRating>()
            .HasOne(static r => r.User)
            .WithMany()
            .HasForeignKey(static r => r.UserId);

        builder.Entity<AlgorithmStatistic>()
            .HasKey(static s => new { s.UserId, s.AlgorithmId });
        builder.Entity<AlgorithmStatistic>()
            .HasOne(static s => s.Algorithm)
            .WithMany()
            .HasForeignKey(static s => s.AlgorithmId);
        builder.Entity<AlgorithmStatistic>()
            .HasOne(static s => s.User)
            .WithMany()
            .HasForeignKey(static s => s.UserId);
        builder.Entity<AlgorithmStatistic>()
            .Property(static s => s.TotalTimeSolvingInSeconds)
            .HasColumnType("DECIMAL(10, 2)");
        builder.Entity<AlgorithmStatistic>()
            .Property(static s => s.BestTimeInSeconds)
            .HasColumnType("DECIMAL(10, 2)");

        builder.Entity<Case>()
            .Property(static c => c.Name)
            .HasMaxLength(100);
        builder.Entity<Case>()
            .Property(static c => c.ImageUrl)
            .HasMaxLength(500);
        builder.Entity<Case>()
            .Property(static c => c.DefaultSolution)
            .HasMaxLength(100);
        builder.Entity<Case>()
            .Property(static c => c.Type)
            .HasConversion(
                static t => t.ToString(),
                static s => Enum.Parse<CaseType>(s)
            )
            .HasMaxLength(50);

        builder.Entity<TrainingPlan>()
            .HasOne(static p => p.User)
            .WithMany()
            .HasForeignKey(static p => p.UserId);
        builder.Entity<TrainingPlan>()
            .Property(static p => p.Name)
            .HasMaxLength(100);

        builder.Entity<TrainingPlanCase>()
            .HasKey(static c => new { c.TrainingPlanId, c.CaseId });
        builder.Entity<TrainingPlanCase>()
            .HasOne(static c => c.TrainingPlan)
            .WithMany(static p => p.TrainingPlanCases)
            .HasForeignKey(static c => c.TrainingPlanId);
        builder.Entity<TrainingPlanCase>()
            .HasOne(static c => c.Case)
            .WithMany()
            .HasForeignKey(static c => c.CaseId);
        builder.Entity<TrainingPlanCase>()
            .Property(static c => c.LastDifficultyRating)
            .HasConversion(
                static r => r.ToString(),
                static s => s == null ? null : Enum.Parse<DifficultyRating>(s)
            )
            .HasMaxLength(50);
        builder.Entity<TrainingPlanCase>()
            .Property(static c => c.LastSolved)
            .HasColumnType("TIMESTAMP");

        builder.Entity<UserCase>()
            .HasKey(static c => new { c.UserId, c.CaseId });
        builder.Entity<UserCase>()
            .HasOne(static c => c.User)
            .WithMany()
            .HasForeignKey(static c => c.UserId);
        builder.Entity<UserCase>()
            .HasOne(static c => c.Case)
            .WithMany()
            .HasForeignKey(static c => c.CaseId);
        builder.Entity<UserCase>()
            .Property(static c => c.Status)
            .HasConversion(
                static s => s.ToString(),
                static s => Enum.Parse<UserCaseStatus>(s)
            )
            .HasMaxLength(50);
    }
}