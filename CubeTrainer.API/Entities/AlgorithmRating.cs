namespace CubeTrainer.API.Entities;

internal sealed class AlgorithmRating
{
    public Guid AlgorithmId { get; set; }

    public Algorithm Algorithm { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public User User { get; set; } = default!;

    public int Rating { get; set; }
}