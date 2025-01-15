namespace CubeTrainer.API.Entities;

internal sealed class Case
{
    public Guid Id { get; set; }

    // NOTE: might delete this property later
    public CaseType Type { get; set; }

    public string Name { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;
}