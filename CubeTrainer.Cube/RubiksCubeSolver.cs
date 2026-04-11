using CubeTrainer.Cube.Kociemba;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;

namespace CubeTrainer.Cube;

public static class RubiksCubeSolver
{
    public static MoveSequence FindSolution(RubiksCube cube)
    {
        var solution = Solver.Solve(
            cube.CornerOrientationCoordinate,
            cube.EdgeOrientationCoordinate,
            cube.UDSliceCoordinatePhase1,
            cube.CornerPermutationCoordinate,
            cube.EdgePermutationCoordinate,
            cube.UDSliceCoordinatePhase2);
        return new([.. solution.Select(static m => new Move(m.Face, m.Count))]);
    }
}