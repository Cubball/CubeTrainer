using BenchmarkDotNet.Attributes;
using CubeTrainer.Cube;

namespace CubeTrainer.Benchmarks;

[MemoryDiagnoser]
public class SolverBenchmarks
{
    private const int CubeCount = 100;

    private RubiksCube[] _cubes = null!;

    [Params(10, 25, 100)]
    public int ScrambleLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var warmup = RubiksCube.Scrambled(MoveSequence.Random(1));
        RubiksCubeSolver.FindSolution(warmup);

        _cubes = new RubiksCube[CubeCount];
        for (var i = 0; i < CubeCount; i++)
        {
            _cubes[i] = RubiksCube.Scrambled(MoveSequence.Random(ScrambleLength));
        }
    }

    [Benchmark]
    public void Solve100Cubes()
    {
        for (var i = 0; i < CubeCount; i++)
        {
            RubiksCubeSolver.FindSolution(_cubes[i]);
        }
    }
}