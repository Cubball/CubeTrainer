namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record RunSummary(
    int Run,
    int TotalExectMatches,
    double WorstCasePrefixScore,
    double TotalCost);
