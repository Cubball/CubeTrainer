namespace CubeTrainer.Cube.Analyzer.Models;

public abstract record AnalysisStep(AnalysisStepCost Cost);

public sealed record MotionAnalysisStep(
    string Move,
    string Hand,
    string MotionType,
    AnalysisStepCost Cost) : AnalysisStep(Cost);

public sealed record RegripAnalysisStep(
    string Hand,
    string NewHandOffset,
    AnalysisStepCost Cost) : AnalysisStep(Cost);

public sealed record RotationAnalysisStep(
    string Move,
    AnalysisStepCost Cost) : AnalysisStep(Cost);
