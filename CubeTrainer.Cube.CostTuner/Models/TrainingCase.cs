namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record TrainingCase(
    string Name,
    string Algorithm,
    IReadOnlyList<string> GoalTokens);
