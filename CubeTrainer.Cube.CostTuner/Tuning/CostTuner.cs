using System.Reflection;
using CubeTrainer.Cube;
using CubeTrainer.Cube.Analyzer;
using CubeTrainer.Cube.CostTuner.Models;

namespace CubeTrainer.Cube.CostTuner.Tuning;

internal sealed class CostTuner
{
    private const int PopulationSize = 120;
    private const int MaxNoImprovementGenerations = 100;
    private const int EliteCount = 6;
    private const int TournamentSize = 5;
    private const int MaxNewChildAttempts = 3000;
    private const double MutationChance = 0.5;
    private const double DeltaMax = 6.0;
    private const double MinValue = 0.01;
    private const double MaxValue = 12.0;
    private const double InitialJitterRange = 0.75;

    private readonly IReadOnlyList<TrainingCase> _trainingCases;
    private readonly IReadOnlyList<CostConstraint> _constraints;
    private readonly List<PropertyInfo> _tunableProperties;
    private readonly Dictionary<string, PropertyInfo> _propertyByName;
    private readonly Random _random = new();

    public CostTuner(IReadOnlyList<TrainingCase> trainingCases, IReadOnlyList<CostConstraint> constraints)
    {
        _trainingCases = trainingCases;
        _constraints = constraints;
        _tunableProperties = GetTunableProperties();
        _propertyByName = _tunableProperties.ToDictionary(static p => p.Name, StringComparer.Ordinal);
    }

    public TuneResult Tune()
    {
        var population = new List<Individual>(PopulationSize);
        var initialAttempts = 0;

        while (population.Count < PopulationSize)
        {
            initialAttempts++;
            var candidate = CreateRandomConfig();
            if (!AreConstraintsSatisfied(candidate))
            {
                continue;
            }

            population.Add(new Individual(candidate, Evaluate(candidate)));
        }

        Console.WriteLine($"Generated initial population: size={population.Count}, attempts={initialAttempts}");
        var bestOverall = SelectBest(population).Result;
        Console.WriteLine($"Initial best: exactMatches={bestOverall.TotalExectMatches}, worst={bestOverall.WorstCasePrefixScore:0.###}, cost={bestOverall.TotalCost:0.###}");

        var generation = 1;
        var generationsWithoutImprovement = 0;
        while (generationsWithoutImprovement < MaxNoImprovementGenerations)
        {
            var elites = SelectElites(population);
            var nextPopulation = new List<Individual>(PopulationSize);
            nextPopulation.AddRange(elites);

            var attempts = 0;
            while (nextPopulation.Count < PopulationSize && attempts < MaxNewChildAttempts)
            {
                attempts++;
                var parentA = TournamentSelect(population);
                var parentB = TournamentSelect(population);

                var childConfig = Crossover(parentA.Config, parentB.Config);
                Mutate(childConfig);
                if (!AreConstraintsSatisfied(childConfig))
                {
                    continue;
                }

                var childResult = Evaluate(childConfig);
                nextPopulation.Add(new Individual(childConfig, childResult));
            }

            var fallbackAttempts = 0;
            while (nextPopulation.Count < PopulationSize)
            {
                fallbackAttempts++;
                if (fallbackAttempts > MaxNewChildAttempts)
                {
                    var fillWith = elites.Count > 0 ? elites[_random.Next(elites.Count)] : population[_random.Next(population.Count)];
                    nextPopulation.Add(new Individual(fillWith.Config.Clone(), fillWith.Result));
                    continue;
                }

                var fallback = CreateRandomNeighbor(bestOverall.Config);
                Mutate(fallback);
                if (!AreConstraintsSatisfied(fallback))
                {
                    continue;
                }

                nextPopulation.Add(new Individual(fallback, Evaluate(fallback)));
            }

            population = nextPopulation;
            var generationBest = SelectBest(population).Result;
            var hasValueImproved = generationBest.TotalExectMatches > bestOverall.TotalExectMatches;
            if (hasValueImproved)
            {
                bestOverall = generationBest;
                generationsWithoutImprovement = 0;
            }
            else
            {
                generationsWithoutImprovement++;
                if (generationBest.TotalExectMatches == bestOverall.TotalExectMatches && _random.Next(2) == 0)
                {
                    bestOverall = generationBest;
                }
            }

            var averageMatches = population.Average(static p => p.Result.TotalExectMatches);
            Console.WriteLine($"Gen {generation:000}: best={generationBest.TotalExectMatches}, avg={averageMatches:0.###}, overall={bestOverall.TotalExectMatches}, stagnant={generationsWithoutImprovement}/{MaxNoImprovementGenerations}, mutationChance={MutationChance:0.00}, dmax={DeltaMax:0.0}, cost={bestOverall.TotalCost:0.###}");
            generation++;
        }

        return bestOverall;
    }

    private List<Individual> SelectElites(List<Individual> population)
    {
        return population
            .OrderBy(_ => _random.Next())
            .OrderByDescending(static p => p.Result.TotalExectMatches)
            .Take(EliteCount)
            .Select(static p => new Individual(p.Config.Clone(), p.Result))
            .ToList();
    }

    private Individual TournamentSelect(List<Individual> population)
    {
        var winner = population[_random.Next(population.Count)];
        for (var i = 1; i < TournamentSize; i++)
        {
            var challenger = population[_random.Next(population.Count)];
            if (IsBetter(challenger.Result, winner.Result))
            {
                winner = challenger;
            }
        }

        return winner;
    }

    private Individual SelectBest(List<Individual> population)
    {
        var best = population[0];
        for (var i = 1; i < population.Count; i++)
        {
            if (IsBetter(population[i].Result, best.Result))
            {
                best = population[i];
            }
        }

        return best;
    }

    private CostConfig Crossover(CostConfig parentA, CostConfig parentB)
    {
        var child = parentA.Clone();
        foreach (var property in _tunableProperties)
        {
            var a = (double)property.GetValue(parentA)!;
            var b = (double)property.GetValue(parentB)!;
            var selected = _random.Next(2) == 0 ? a : b;
            property.SetValue(child, Clamp(selected, MinValue, MaxValue));
        }

        return child;
    }

    private void Mutate(CostConfig config)
    {
        if (_random.NextDouble() > MutationChance)
        {
            return;
        }

        var property = _tunableProperties[_random.Next(_tunableProperties.Count)];
        var current = (double)property.GetValue(config)!;
        var delta = ((_random.NextDouble() * 2.0) - 1.0) * DeltaMax;
        property.SetValue(config, Clamp(current + delta, MinValue, MaxValue));
    }

    private TuneResult Evaluate(CostConfig costConfig)
    {
        var exactMatchCount = 0;
        var totalCost = 0.0;
        var worstCasePrefixScore = double.MaxValue;
        var outcomes = new List<CaseOutcome>(_trainingCases.Count);

        foreach (var trainingCase in _trainingCases)
        {
            var result = CubeTrainer.Cube.Analyzer.Analyzer.Analyze(MoveSequence.FromString(trainingCase.Algorithm), costConfig);
            var actualTokens = StepTokenizer.TokenizeSteps(result.Steps);
            var prefixLength = CountCommonPrefix(trainingCase.GoalTokens, actualTokens);
            var ratio = trainingCase.GoalTokens.Count == 0
                ? 0.0
                : (double)prefixLength / trainingCase.GoalTokens.Count;
            var isExactMatch = prefixLength == trainingCase.GoalTokens.Count && actualTokens.Count == trainingCase.GoalTokens.Count;
            exactMatchCount += isExactMatch ? 1 : 0;

            totalCost += result.TotalCost;
            worstCasePrefixScore = Math.Min(worstCasePrefixScore, ratio);

            var expectedTokenAtMismatch = prefixLength < trainingCase.GoalTokens.Count
                ? trainingCase.GoalTokens[prefixLength]
                : null;
            var actualTokenAtMismatch = prefixLength < actualTokens.Count
                ? actualTokens[prefixLength]
                : null;

            outcomes.Add(new CaseOutcome(
                trainingCase.Name,
                prefixLength,
                trainingCase.GoalTokens.Count,
                result.TotalCost,
                expectedTokenAtMismatch,
                actualTokenAtMismatch));
        }

        if (worstCasePrefixScore == double.MaxValue)
        {
            worstCasePrefixScore = 0.0;
        }

        return new TuneResult(costConfig.Clone(), exactMatchCount, worstCasePrefixScore, totalCost, outcomes);
    }

    private bool AreConstraintsSatisfied(CostConfig config)
    {
        foreach (var constraint in _constraints)
        {
            var left = GetPropertyValue(config, constraint.LeftProperty);
            var right = GetPropertyValue(config, constraint.RightProperty);
            var valid = constraint.Relation switch
            {
                ConstraintRelation.GreaterThan => left > right,
                ConstraintRelation.GreaterOrEqual => left >= right,
                ConstraintRelation.LessThan => left < right,
                ConstraintRelation.LessOrEqual => left <= right,
                _ => false,
            };

            if (!valid)
            {
                return false;
            }
        }

        return true;
    }

    private CostConfig CreateRandomNeighbor(CostConfig baseConfig)
    {
        var candidate = baseConfig.Clone();

        foreach (var property in _tunableProperties)
        {
            var currentValue = (double)property.GetValue(candidate)!;
            var jitter = 1.0 + ((_random.NextDouble() - 0.5) * 2.0 * InitialJitterRange);
            var randomized = Clamp(currentValue * jitter, MinValue, MaxValue);
            property.SetValue(candidate, randomized);
        }

        return candidate;
    }

    private CostConfig CreateRandomConfig()
    {
        var config = new CostConfig();
        foreach (var property in _tunableProperties)
        {
            var value = MinValue + (_random.NextDouble() * (MaxValue - MinValue));
            property.SetValue(config, value);
        }

        return config;
    }

    private bool IsBetter(TuneResult candidate, TuneResult baseline)
    {
        if (candidate.TotalExectMatches != baseline.TotalExectMatches)
        {
            return candidate.TotalExectMatches > baseline.TotalExectMatches;
        }

        return _random.Next(2) == 0;
    }

    private static int CountCommonPrefix(IReadOnlyList<string> expected, IReadOnlyList<string> actual)
    {
        var count = Math.Min(expected.Count, actual.Count);
        var prefixLength = 0;
        for (var i = 0; i < count; i++)
        {
            if (!string.Equals(expected[i], actual[i], StringComparison.Ordinal))
            {
                break;
            }

            prefixLength++;
        }

        return prefixLength;
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Min(max, Math.Max(min, value));
    }

    private PropertyInfo GetProperty(string propertyName)
    {
        if (_propertyByName.TryGetValue(propertyName, out var property))
        {
            return property;
        }

        throw new InvalidOperationException($"Unknown cost property: {propertyName}");
    }

    private double GetPropertyValue(CostConfig config, string propertyName)
    {
        var property = GetProperty(propertyName);
        return (double)property.GetValue(config)!;
    }

    public static List<PropertyInfo> GetTunableProperties()
    {
        return typeof(CostConfig)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(static p => p.PropertyType == typeof(double) && p.CanRead && p.CanWrite)
            .OrderBy(static p => p.Name, StringComparer.Ordinal)
            .ToList();
    }
}
