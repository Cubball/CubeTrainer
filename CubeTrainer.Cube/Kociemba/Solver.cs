using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Infrastructure;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;
using UDSliceCoordinatePhase1 = CubeTrainer.Cube.Kociemba.Phase1.Coordinates.UDSliceCoordinate;
using UDSliceCoordinatePhase2 = CubeTrainer.Cube.Kociemba.Phase2.Coordinates.UDSliceCoordinate;

namespace CubeTrainer.Cube.Kociemba;

internal static class Solver
{
    private static readonly PhaseSolver<CornerOrientationCoordinate, EdgeOrientationCoordinate, UDSliceCoordinatePhase1> Phase1Solver = new(
        Constants.Phase1Moves,
        Constants.Phase1MaxDepth,
        FileManager.LoadMoveTableFromFile<CornerOrientationCoordinate>(FileManager.COMoveTablePath),
        FileManager.LoadMoveTableFromFile<EdgeOrientationCoordinate>(FileManager.EOMoveTablePath),
        FileManager.LoadMoveTableFromFile<UDSliceCoordinatePhase1>(FileManager.UDSlicePhase1MoveTablePath),
        FileManager.LoadPruneTableFromFile<CornerOrientationCoordinate, UDSliceCoordinatePhase1>(FileManager.COPruneTablePath),
        FileManager.LoadPruneTableFromFile<EdgeOrientationCoordinate, UDSliceCoordinatePhase1>(FileManager.EOPruneTablePath));
    private static readonly PhaseSolver<CornerPermutationCoordinate, EdgePermutationCoordinate, UDSliceCoordinatePhase2> Phase2Solver = new(
        Constants.Phase2Moves,
        Constants.Phase2MaxDepth,
        FileManager.LoadMoveTableFromFile<CornerPermutationCoordinate>(FileManager.CPMoveTablePath),
        FileManager.LoadMoveTableFromFile<EdgePermutationCoordinate>(FileManager.EPMoveTablePath),
        FileManager.LoadMoveTableFromFile<UDSliceCoordinatePhase2>(FileManager.UDSlicePhase2MoveTablePath),
        FileManager.LoadPruneTableFromFile<CornerPermutationCoordinate, UDSliceCoordinatePhase2>(FileManager.CPPruneTablePath),
        FileManager.LoadPruneTableFromFile<EdgePermutationCoordinate, UDSliceCoordinatePhase2>(FileManager.EPPruneTablePath));

    public static List<Common.Models.Move> Solve(
        CornerOrientationCoordinate co,
        EdgeOrientationCoordinate eo,
        UDSliceCoordinatePhase1 ud1,
        CornerPermutationCoordinate cp,
        EdgePermutationCoordinate ep,
        UDSliceCoordinatePhase2 ud2)
    {
        var phase1Moves = Phase1Solver.Solve(co.Coordinate, eo.Coordinate, ud1.Coordinate);
        foreach (var move in phase1Moves)
        {
            ((ICoordinate)cp).Apply(move);
            ((ICoordinate)ep).Apply(move);
            ((ICoordinate)ud2).Apply(move);
        }

        var phase2Moves = Phase2Solver.Solve(cp.Coordinate, ep.Coordinate, ud2.Coordinate);
        if (phase1Moves.Count == 0 || phase2Moves.Count == 0)
        {
            return [.. phase1Moves, .. phase2Moves];
        }

        var lastPhase1Move = phase1Moves[^1];
        var firstPhase2Move = phase2Moves[0];
        if (lastPhase1Move.Face != firstPhase2Move.Face)
        {
            return [.. phase1Moves, .. phase2Moves];
        }

        var count = (lastPhase1Move.Count + firstPhase2Move.Count) % 4;
        return count == 0
            ? ([.. phase1Moves.SkipLast(1), .. phase2Moves.Skip(1)])
            : ([.. phase1Moves.SkipLast(1), new(lastPhase1Move.Face, count), .. phase2Moves.Skip(1)]);
    }
}