namespace CubeTrainer.API.Entities;

internal class Algorithm
{
    public Guid Id { get; set; }

    public Guid CaseId { get; set; }

    public Case Case { get; set; } = default!;

    public string Moves { get; set; } = default!;

    public bool IsDeleted { get; set; }

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UsersCount { get; set; }

    public int StarsCount { get; set; }

    public int UsersRatingsCount { get; set; }
}