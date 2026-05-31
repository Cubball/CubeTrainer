using System.Text.Json;
using CubeTrainer.Cube.Analyzer;
using CubeTrainer.Cube.CostTuner.Models;

namespace CubeTrainer.Cube.CostTuner.Infrastructure;

internal static class TunerDataLoader
{
    public static List<TrainingCase> LoadTrainingCases(string path)
    {
        var json = File.ReadAllText(path);
        var payload = JsonSerializer.Deserialize<TrainingDataDocument>(json, JsonOptions())
            ?? throw new InvalidOperationException($"Could not parse training data at '{path}'.");

        if (payload.Cases is null)
        {
            throw new InvalidOperationException($"Training data at '{path}' has no 'cases' array.");
        }

        return payload.Cases
            .Select(static c => new TrainingCase(
                c.Name ?? string.Empty,
                c.Algorithm ?? string.Empty,
                c.ExpectedSteps ?? new List<string>()))
            .ToList();
    }

    public static List<CostConstraint> LoadConstraints(string path)
    {
        var json = File.ReadAllText(path);
        var payload = JsonSerializer.Deserialize<ConstraintDataDocument>(json, JsonOptions())
            ?? throw new InvalidOperationException($"Could not parse constraints data at '{path}'.");

        if (payload.Rules is null)
        {
            throw new InvalidOperationException($"Constraints data at '{path}' has no 'rules' array.");
        }

        return payload.Rules.Select(static rule => new CostConstraint(
                rule.LeftParameter ?? throw new InvalidOperationException("Constraint missing leftParameter."),
                ParseRelation(rule.Operator),
                rule.RightParameter ?? throw new InvalidOperationException("Constraint missing rightParameter.")))
            .ToList();
    }

    public static CostConfig LoadTestCostConfig(string path)
    {
        var json = File.ReadAllText(path);
        var payload = JsonSerializer.Deserialize<TestConfigDocument>(json, JsonOptions())
            ?? throw new InvalidOperationException($"Could not parse test config at '{path}'.");

        if (payload.Config is null)
        {
            throw new InvalidOperationException($"Test config at '{path}' has no 'config' object.");
        }

        var config = CostConfig.Default.Clone();
        var properties = typeof(CostConfig)
            .GetProperties()
            .Where(static p => p.PropertyType == typeof(double) && p.CanWrite)
            .ToDictionary(static p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var (name, value) in payload.Config)
        {
            if (!properties.TryGetValue(name, out var property))
            {
                throw new InvalidOperationException($"Unknown CostConfig field in test.json: '{name}'.");
            }

            property.SetValue(config, value);
        }

        return config;
    }

    private static ConstraintRelation ParseRelation(string? op)
    {
        return op switch
        {
            ">" => ConstraintRelation.GreaterThan,
            ">=" => ConstraintRelation.GreaterOrEqual,
            "<" => ConstraintRelation.LessThan,
            "<=" => ConstraintRelation.LessOrEqual,
            _ => throw new InvalidOperationException($"Unsupported constraint operator: '{op}'."),
        };
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    private sealed class TrainingDataDocument
    {
        public List<TrainingCaseDto>? Cases { get; init; }
    }

    private sealed class TrainingCaseDto
    {
        public string? Name { get; init; }
        public string? Algorithm { get; init; }
        public List<string>? ExpectedSteps { get; init; }
    }

    private sealed class ConstraintDataDocument
    {
        public List<ConstraintRuleDto>? Rules { get; init; }
    }

    private sealed class ConstraintRuleDto
    {
        public string? LeftParameter { get; init; }
        public string? Operator { get; init; }
        public string? RightParameter { get; init; }
    }

    private sealed class TestConfigDocument
    {
        public Dictionary<string, double>? Config { get; init; }
    }
}
