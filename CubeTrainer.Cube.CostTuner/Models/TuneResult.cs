using CubeTrainer.Cube.Analyzer;

namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record TuneResult(
    CostConfig Config,
    int TotalExectMatches,
    double WorstCasePrefixScore,
    double TotalCost,
    List<CaseOutcome> Outcomes);
