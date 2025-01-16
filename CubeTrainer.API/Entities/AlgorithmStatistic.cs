namespace CubeTrainer.API.Entities;

internal sealed class AlgorithmStatistic
{
    public Guid AlgorithmId { get; set; }

    public Algorithm Algorithm { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public decimal TotalTimeSolvingInSeconds { get; set; }

    public int TimedSolvesCount { get; set; }

    public int UntimedSolvesCount { get; set; }

    public decimal? BestTimeInSeconds { get; set; }
}