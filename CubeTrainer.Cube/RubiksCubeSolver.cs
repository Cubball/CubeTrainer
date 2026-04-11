using CubeTrainer.Cube.Kociemba;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;

namespace CubeTrainer.Cube;

public static class RubiksCubeSolver
{
    public static MoveSequence FindSolution(RubiksCube cube)
    {
        // Deep-clone phase 2 coordinates because the solver mutates them
        // when applying phase 1 moves to prepare for phase 2.
        var cp = new CornerPermutationCoordinate(cube.CornerPermutationCoordinate);
        var ep = new EdgePermutationCoordinate(cube.EdgePermutationCoordinate);
        var ud2 = new UDSliceCoordinate(cube.UDSliceCoordinatePhase2);

        var solution = Solver.Solve(
            cube.CornerOrientationCoordinate,
            cube.EdgeOrientationCoordinate,
            cube.UDSliceCoordinatePhase1,
            cp,
            ep,
            ud2);
        return new([.. solution.Select(static m => new Move(m.Face, m.Count))]);
    }
}