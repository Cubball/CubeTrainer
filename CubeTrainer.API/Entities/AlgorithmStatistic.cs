namespace CubeTrainer.API.Entities;

// NOTE: might remove this entity altogether and add
// stats like time solved, number of solves, etc to UserCase
// so it'll be per case, rather than per alg
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