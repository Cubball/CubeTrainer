namespace CubeTrainer.API.Entities;

internal class Case
{
    public Guid Id { get; set; }

    public CaseType Type { get; set; }

    public string Name { get; set; } = default!;

    public string ImageUrl { get; set; } = default!;
}