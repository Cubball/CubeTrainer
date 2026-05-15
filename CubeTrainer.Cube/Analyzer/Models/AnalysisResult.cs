namespace CubeTrainer.Cube.Analyzer.Models;

public sealed record AnalysisResult(
    double TotalCost,
    List<AnalysisStep> Steps);