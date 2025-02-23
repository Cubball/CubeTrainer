using CubeTrainer.Cube;

var scramble = MoveSequence.Random(20);
var cube = RubiksCube.Scrambled(scramble);
var solution = RubiksCubeSolver.FindSolution(cube);
Console.WriteLine($"Scramble: {scramble}");
Console.WriteLine($"Solution: {solution}");