namespace CubeTrainer.Cube.Analyzer.Models;

public sealed record AnalysisStepCost(
    double TotalCost,
    double BaseCost,
    List<CostModifier> Multipliers,
    List<CostModifier> Penalties);