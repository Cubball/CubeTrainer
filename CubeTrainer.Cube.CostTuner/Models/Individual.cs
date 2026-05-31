using CubeTrainer.Cube.Analyzer;

namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record Individual(CostConfig Config, TuneResult Result);
