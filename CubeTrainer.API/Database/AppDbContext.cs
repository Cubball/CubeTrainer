using CubeTrainer.API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CubeTrainer.API.Database;

internal class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Algorithm> Algorithms { get; set; } = default!;

    public DbSet<AlgorithmRating> AlgorithmRatings { get; set; } = default!;

    public DbSet<AlgorithmStatistic> AlgorithmStatistics { get; set; } = default!;

    public DbSet<Case> Cases { get; set; } = default!;

    public DbSet<TrainingPlan> TrainingPlans { get; set; } = default!;

    public DbSet<TrainingPlanCase> TrainingPlanCases { get; set; } = default!;

    public DbSet<UserCase> UserCases { get; set; } = default!;

    // TODO: add enum converters, string lengths, indexes
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Algorithm>()
            .HasOne(static a => a.Case)
            .WithMany()
            .HasForeignKey(static a => a.CaseId);

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

        builder.Entity<TrainingPlan>()
            .HasOne(static p => p.User)
            .WithMany()
            .HasForeignKey(static p => p.UserId);

        builder.Entity<TrainingPlanCase>()
            .HasKey(static c => new { c.TrainingPlanId, c.CaseId });
        builder.Entity<TrainingPlanCase>()
            .HasOne(static c => c.TrainingPlan)
            .WithMany()
            .HasForeignKey(static c => c.TrainingPlanId);
        builder.Entity<TrainingPlanCase>()
            .HasOne(static c => c.Case)
            .WithMany()
            .HasForeignKey(static c => c.CaseId);

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
    }
}