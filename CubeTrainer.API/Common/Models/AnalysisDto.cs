using System.Text.Json.Serialization;
using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.API.Common.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(MotionAnalysisStepDto), "motion")]
[JsonDerivedType(typeof(RegripAnalysisStepDto), "regrip")]
[JsonDerivedType(typeof(RotationAnalysisStepDto), "rotation")]
internal abstract record AnalysisStepDto(AnalysisStepCostDto Cost);

internal sealed record MotionAnalysisStepDto(
    string Move,
    string Hand,
    string MotionType,
    AnalysisStepCostDto Cost) : AnalysisStepDto(Cost);

internal sealed record RegripAnalysisStepDto(
    string Hand,
    string NewHandOffset,
    AnalysisStepCostDto Cost) : AnalysisStepDto(Cost);

internal sealed record RotationAnalysisStepDto(
    string Move,
    AnalysisStepCostDto Cost) : AnalysisStepDto(Cost);

internal sealed record AnalysisResultDto(
    double TotalCost,
    List<AnalysisStepDto> Steps);

internal sealed record AnalysisStepCostDto(
    double TotalCost,
    double BaseCost,
    List<CostModifierDto> Multipliers,
    List<CostModifierDto> Penalties);

internal sealed record CostModifierDto(double Value, string Description);

internal static class AnalysisMapper
{
    public static AnalysisResultDto ToDto(AnalysisResult result)
    {
        return new AnalysisResultDto(
            result.TotalCost,
            [.. result.Steps.Select(MapStep)]);
    }

    private static AnalysisStepDto MapStep(AnalysisStep step) => step switch
    {
        MotionAnalysisStep m => new MotionAnalysisStepDto(m.Move, m.Hand, m.MotionType, MapCost(m.Cost)),
        RegripAnalysisStep r => new RegripAnalysisStepDto(r.Hand, r.NewHandOffset, MapCost(r.Cost)),
        RotationAnalysisStep r => new RotationAnalysisStepDto(r.Move, MapCost(r.Cost)),
        _ => throw new InvalidOperationException($"Unknown analysis step type: {step.GetType().Name}"),
    };

    private static AnalysisStepCostDto MapCost(AnalysisStepCost cost)
    {
        return new AnalysisStepCostDto(
            cost.TotalCost,
            cost.BaseCost,
            [.. cost.Multipliers.Select(static m => new CostModifierDto(m.Value, m.Description))],
            [.. cost.Penalties.Select(static p => new CostModifierDto(p.Value, p.Description))]);
    }
}
