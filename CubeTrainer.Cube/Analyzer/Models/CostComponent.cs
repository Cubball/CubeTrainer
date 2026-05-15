namespace CubeTrainer.Cube.Analyzer.Models;

public sealed record CostComponent(
    double BaseCost,
    List<(double Value, string Description)> Multipliers,
    List<(double Value, string Description)> Penalties)
{
    public double TotalCost
    {
        get
        {
            var total = BaseCost;
            foreach (var (value, _) in Multipliers)
            {
                total *= value;
            }

            foreach (var (value, _) in Penalties)
            {
                total += value;
            }

            return total;
        }
    }

    public override string ToString()
    {
        var multipliers = string.Join(" * ", Multipliers.Select(m => $"{m.Value} ({m.Description})"));
        var penalties = string.Join(" + ", Penalties.Select(p => $"{p.Value} ({p.Description})"));
        if (multipliers.Length > 0)
        {
            multipliers = $" * {multipliers}";
        }

        if (penalties.Length > 0)
        {
            penalties = $" + {penalties}";
        }

        return $"Total Cost: {TotalCost} [{BaseCost} (base){multipliers}{penalties}]";
    }
}
