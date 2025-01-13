namespace CubeTrainer.API.Entities;

internal class TrainingPlan
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public string Name { get; set; } = default!;
}