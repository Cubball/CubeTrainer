namespace CubeTrainer.API.Entities;

internal class TrainingPlanCase
{
    public Guid TrainingPlanId { get; set; }

    public TrainingPlan TrainingPlan { get; set; } = default!;

    public Guid CaseId { get; set; }

    public Case Case { get; set; } = default!;

    public int SolvesToLearnCount { get; set; }

    public DifficultyRating LastDifficultyRating { get; set; }

    public DateTime? LastSolved { get; set; }
}