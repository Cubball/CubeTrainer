using CubeTrainer.Cube;
using Xunit;

namespace CubeTrainer.Cube.Tests;

public class RubiksCubeSolverTests
{
    [Fact]
    public void SolvedCube_IsSolved()
    {
        var cube = RubiksCube.Solved();

        Assert.True(cube.IsSolved);
    }

    [Fact]
    public void ScrambledCube_IsNotSolved()
    {
        var scramble = MoveSequence.FromString("R U F");
        var cube = RubiksCube.Scrambled(scramble);

        Assert.False(cube.IsSolved);
    }

    [Theory]
    [InlineData("R U R' U'")]
    [InlineData("R U F B L D")]
    [InlineData("R2 U2 F2 B2 L2 D2")]
    [InlineData("R U' L' B2 D F R' U L D'")]
    [InlineData("F R U R' U' F'")]
    public void FindSolution_WithKnownScramble_ProducesSolvingSequence(string scrambleStr)
    {
        var scramble = MoveSequence.FromString(scrambleStr);
        var cube = RubiksCube.Scrambled(scramble);

        var solution = RubiksCubeSolver.FindSolution(cube);
        cube.Apply(solution);

        Assert.True(cube.IsSolved,
            $"Cube should be solved after applying solution.\nScramble: {scramble}\nSolution: {solution}");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(25)]
    [InlineData(100)]
    public void FindSolution_WithRandomScramble_ProducesSolvingSequence(int scrambleLength)
    {
        var scramble = MoveSequence.Random(scrambleLength);
        var cube = RubiksCube.Scrambled(scramble);

        var solution = RubiksCubeSolver.FindSolution(cube);
        cube.Apply(solution);

        Assert.True(cube.IsSolved,
            $"Cube should be solved after applying solution.\nScramble: {scramble}\nSolution: {solution}");
    }

    [Fact]
    public void FindSolution_MultipleRandomScrambles_AllSolveCorrectly()
    {
        const int iterations = 10;

        for (var i = 0; i < iterations; i++)
        {
            var scrambleLength = Random.Shared.Next(5, 26);
            var scramble = MoveSequence.Random(scrambleLength);
            var cube = RubiksCube.Scrambled(scramble);

            var solution = RubiksCubeSolver.FindSolution(cube);
            cube.Apply(solution);

            Assert.True(cube.IsSolved,
                $"Iteration {i}: Cube should be solved after applying solution.\nScramble: {scramble}\nSolution: {solution}");
        }
    }

    [Fact]
    public void FindSolution_SolvedCube_ReturnsEmptyOrValidSolution()
    {
        var cube = RubiksCube.Solved();

        var solution = RubiksCubeSolver.FindSolution(cube);
        cube.Apply(solution);

        Assert.True(cube.IsSolved,
            $"Already-solved cube should remain solved.\nSolution: {solution}");
    }

    [Fact]
    public void FindSolution_SingleMove_ProducesSolvingSequence()
    {
        var scramble = MoveSequence.FromString("R");
        var cube = RubiksCube.Scrambled(scramble);

        var solution = RubiksCubeSolver.FindSolution(cube);
        cube.Apply(solution);

        Assert.True(cube.IsSolved,
            $"Cube should be solved after applying solution.\nScramble: {scramble}\nSolution: {solution}");
    }

    [Fact]
    public void Apply_ScrambleThenInverse_CubeIsSolved()
    {
        var scramble = MoveSequence.Random(20);
        var cube = RubiksCube.Scrambled(scramble);

        cube.Apply(scramble.Inverse());

        Assert.True(cube.IsSolved,
            $"Applying a scramble followed by its inverse should result in a solved cube.\nScramble: {scramble}");
    }
}