namespace CubeTrainer.API.Entities;

internal sealed class Case
{
    public Guid Id { get; set; }

    public CaseType Type { get; set; }

    public string Name { get; set; } = default!;

    public string DefaultSolution { get; set; } = default!;
}