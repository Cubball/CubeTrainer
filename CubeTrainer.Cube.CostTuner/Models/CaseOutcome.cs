namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record CaseOutcome(
    string Name,
    int PrefixLength,
    int GoalLength,
    double TotalCost,
    string? ExpectedTokenAtMismatch,
    string? ActualTokenAtMismatch);
