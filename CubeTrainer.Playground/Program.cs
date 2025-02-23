using System.Diagnostics;
using CubeTrainer.Cube;

var scramble = MoveSequence.Random(20);
var cube = RubiksCube.Scrambled(scramble);
var stopwatch = Stopwatch.StartNew();
var solution = RubiksCubeSolver.FindSolution(cube);
stopwatch.Stop();
Console.WriteLine($"Scramble: {scramble}");
Console.WriteLine($"Solution: {solution}");
Console.WriteLine($"Time: {stopwatch.Elapsed}");