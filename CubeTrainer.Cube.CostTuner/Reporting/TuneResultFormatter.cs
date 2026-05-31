using CubeTrainer.Cube.CostTuner.Models;
using CostTunerEngine = CubeTrainer.Cube.CostTuner.Tuning.CostTuner;

namespace CubeTrainer.Cube.CostTuner.Reporting;

internal static class TuneResultFormatter
{
    public static JsonReport BuildJsonReport(TuneResult tuned, IReadOnlyList<RunSummary> runs)
    {
        var config = CostTunerEngine.GetTunableProperties()
            .ToDictionary(
                static p => p.Name,
                p => (double)p.GetValue(tuned.Config)!,
                StringComparer.Ordinal);

        var outcomes = tuned.Outcomes.Select(static outcome =>
        {
            var ratio = outcome.GoalLength == 0 ? 0.0 : (double)outcome.PrefixLength / outcome.GoalLength;
            return new JsonCaseOutcome(
                outcome.Name,
                outcome.PrefixLength,
                outcome.GoalLength,
                ratio,
                outcome.TotalCost,
                outcome.ExpectedTokenAtMismatch,
                outcome.ActualTokenAtMismatch);
        }).ToList();

        var jsonRuns = runs.Select(static run => new JsonRunSummary(
            run.Run,
            run.TotalExectMatches,
            run.WorstCasePrefixScore,
            run.TotalCost)).ToList();

        return new JsonReport(
            DateTime.UtcNow,
            runs.Count,
            new JsonBestResult(
                tuned.TotalExectMatches,
                tuned.WorstCasePrefixScore,
                tuned.TotalCost,
                config,
                outcomes),
            jsonRuns);
    }
}

internal sealed record JsonReport(
    DateTime GeneratedAtUtc,
    int RandomStartRuns,
    JsonBestResult Best,
    List<JsonRunSummary> Runs);

internal sealed record JsonBestResult(
    int TotalExectMatches,
    double WorstCasePrefixScore,
    double TotalCost,
    Dictionary<string, double> Config,
    List<JsonCaseOutcome> Outcomes);

internal sealed record JsonRunSummary(
    int Run,
    int TotalExectMatches,
    double WorstCasePrefixScore,
    double TotalCost);

internal sealed record JsonCaseOutcome(
    string Name,
    int PrefixLength,
    int GoalLength,
    double Ratio,
    double TotalCost,
    string? ExpectedTokenAtMismatch,
    string? ActualTokenAtMismatch);
