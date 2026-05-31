using CubeTrainer.Cube;
using CubeAnalyzer = CubeTrainer.Cube.Analyzer.Analyzer;
using AnalyzerCostConfig = CubeTrainer.Cube.Analyzer.CostConfig;
using CubeTrainer.Cube.CostTuner.Infrastructure;
using CubeTrainer.Cube.CostTuner.Models;
using CubeTrainer.Cube.CostTuner.Reporting;
using CubeTrainer.Cube.CostTuner.Tuning;
using CubeTrainer.Cube.CostTuner;

var projectDirectory = ResolveProjectDirectory();
var trainingPath = Path.Combine(projectDirectory, "training.json");
var constraintsPath = Path.Combine(projectDirectory, "constraints.json");
var jsonOutputPath = Path.Combine(projectDirectory, "output.json");
var testConfigPath = Path.Combine(projectDirectory, "test.json");

var trainingCases = TunerDataLoader.LoadTrainingCases(trainingPath);

if (!EnsureTrainingCasesHaveGoalTokens(trainingCases))
{
    return;
}

var mode = args.Length == 0 ? "tune" : args[0].Trim().ToLowerInvariant();
switch (mode)
{
    case "tune":
    {
        var constraints = TunerDataLoader.LoadConstraints(constraintsPath);
        RunTuneMode(trainingCases, constraints, jsonOutputPath);
        break;
    }
    case "test":
    {
        var testConfig = TunerDataLoader.LoadTestCostConfig(testConfigPath);
        RunTestMode(trainingCases, testConfig);
        break;
    }
    default:
        Console.WriteLine($"Unknown mode: {mode}");
        Console.WriteLine("Usage: dotnet run -- [tune|test]");
        break;
}

static void RunTuneMode(IReadOnlyList<TrainingCase> trainingCases, IReadOnlyList<CostConstraint> constraints, string jsonOutputPath)
{
    var tuner = new CostTuner(trainingCases, constraints);
    const int randomStartRuns = 50;
    TuneResult? tuned = null;
    var runSummaries = new List<RunSummary>(randomStartRuns);

    for (var run = 1; run <= randomStartRuns; run++)
    {
        Console.WriteLine();
        Console.WriteLine($"=== Run {run}/{randomStartRuns} ===");

        var runResult = tuner.Tune();
        runSummaries.Add(new RunSummary(run, runResult.TotalExectMatches, runResult.WorstCasePrefixScore, runResult.TotalCost));
        Console.WriteLine($"Run {run} result: exactMatches={runResult.TotalExectMatches}, worst={runResult.WorstCasePrefixScore:0.###}, cost={runResult.TotalCost:0.###}");

        if (tuned is null || IsBetterOverall(runResult, tuned))
        {
            tuned = runResult;
            Console.WriteLine($"Run {run} is current best.");
            foreach (var property in CostTuner.GetTunableProperties())
            {
                var value = (double)property.GetValue(tuned.Config)!;
                Console.WriteLine($"  {property.Name} = {value:0.###}");
            }
        }
    }

    tuned ??= tuner.Tune();

    var jsonReport = TuneResultFormatter.BuildJsonReport(tuned, runSummaries);

    TunerOutputWriter.WriteJsonReport(jsonOutputPath, jsonReport);

    Console.WriteLine();
    Console.WriteLine($"Wrote JSON report to: {jsonOutputPath}");
}

static void RunTestMode(IReadOnlyList<TrainingCase> trainingCases, AnalyzerCostConfig costConfig)
{
    var matched = new List<string>();
    var unmatched = new List<(string Name, IReadOnlyList<string> Expected, IReadOnlyList<string> Actual)>();

    foreach (var trainingCase in trainingCases)
    {
        var result = CubeAnalyzer.Analyze(MoveSequence.FromString(trainingCase.Algorithm), costConfig);
        var actualTokens = StepTokenizer.TokenizeSteps(result.Steps);
        var isExactMatch = trainingCase.GoalTokens.Count == actualTokens.Count
            && trainingCase.GoalTokens.SequenceEqual(actualTokens, StringComparer.Ordinal);

        if (isExactMatch)
        {
            matched.Add(trainingCase.Name);
            continue;
        }

        unmatched.Add((trainingCase.Name, trainingCase.GoalTokens, actualTokens));
    }

    Console.WriteLine($"Matched {matched.Count}/{trainingCases.Count} algorithms.");
    Console.WriteLine();
    Console.WriteLine("Matching algorithms:");
    if (matched.Count == 0)
    {
        Console.WriteLine("  <none>");
    }
    else
    {
        foreach (var name in matched)
        {
            Console.WriteLine($"  {name}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("Non-matching algorithms:");
    if (unmatched.Count == 0)
    {
        Console.WriteLine("  <none>");
        return;
    }

    foreach (var caseResult in unmatched)
    {
        Console.WriteLine($"  {caseResult.Name}");
        Console.WriteLine("    expected:");
        PrintExecution(caseResult.Expected, "      ");
        Console.WriteLine("    actual:");
        PrintExecution(caseResult.Actual, "      ");
        Console.WriteLine();
    }
}

static bool EnsureTrainingCasesHaveGoalTokens(IReadOnlyList<TrainingCase> trainingCases)
{
    if (!trainingCases.Any(static t => t.GoalTokens.Count == 0))
    {
        return true;
    }

    Console.WriteLine("One or more training cases has empty GoalTokens.");
    Console.WriteLine("Fill GoalTokens first. Current analyzer output tokens:");
    Console.WriteLine();

    foreach (var trainingCase in trainingCases)
    {
        var result = CubeAnalyzer.Analyze(MoveSequence.FromString(trainingCase.Algorithm), AnalyzerCostConfig.Default);
        var tokens = StepTokenizer.TokenizeSteps(result.Steps);

        Console.WriteLine($"[{trainingCase.Name}] {trainingCase.Algorithm}");
        for (var i = 0; i < tokens.Count; i++)
        {
            Console.WriteLine($"  {i:00}: {tokens[i]}");
        }

        Console.WriteLine();
    }

    return false;
}

static void PrintExecution(IReadOnlyList<string> tokens, string indent)
{
    if (tokens.Count == 0)
    {
        Console.WriteLine($"{indent}<empty>");
        return;
    }

    for (var i = 0; i < tokens.Count; i++)
    {
        Console.WriteLine($"{indent}{i:00}: {tokens[i]}");
    }
}

static bool IsBetterOverall(TuneResult candidate, TuneResult baseline)
{
    if (candidate.TotalExectMatches != baseline.TotalExectMatches)
    {
        return candidate.TotalExectMatches > baseline.TotalExectMatches;
    }

    return Random.Shared.Next(2) == 0;
}

static string ResolveProjectDirectory()
{
    var candidate = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    if (File.Exists(Path.Combine(candidate, "CubeTrainer.Cube.CostTuner.csproj")))
    {
        return candidate;
    }

    return Environment.CurrentDirectory;
}
