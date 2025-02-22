using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase2;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;
using UDSliceCoordinatePhase1 = CubeTrainer.Cube.Kociemba.Phase1.Coordinates.UDSliceCoordinate;
using UDSliceCoordinatePhase2 = CubeTrainer.Cube.Kociemba.Phase2.Coordinates.UDSliceCoordinate;

namespace CubeTrainer.Cube.Kociemba;

internal class Solver(
    MoveTable<CornerOrientationCoordinate> coMoveTable,
    MoveTable<EdgeOrientationCoordinate> eoMoveTable,
    MoveTable<UDSliceCoordinatePhase1> udSlicePhase1MoveTable,
    PruneTable<CornerOrientationCoordinate, UDSliceCoordinatePhase1> coPruneTable,
    PruneTable<EdgeOrientationCoordinate, UDSliceCoordinatePhase1> eoPruneTable,
    MoveTable<CornerPermutationCoordinate> cpMoveTable,
    MoveTable<EdgePermutationCoordinate> epMoveTable,
    MoveTable<UDSliceCoordinatePhase2> udSlicePhase2MoveTable,
    PruneTable<CornerPermutationCoordinate, UDSliceCoordinatePhase2> cpPruneTable,
    PruneTable<EdgePermutationCoordinate, UDSliceCoordinatePhase2> epPruneTable)
{
    private readonly Phase1Solver _phase1Solver = new(
        coMoveTable,
        eoMoveTable,
        udSlicePhase1MoveTable,
        coPruneTable,
        eoPruneTable);
    private readonly Phase2Solver _phase2Solver = new(
        cpMoveTable,
        epMoveTable,
        udSlicePhase2MoveTable,
        cpPruneTable,
        epPruneTable);

    public Move[] Solve(
        CornerOrientationCoordinate co,
        EdgeOrientationCoordinate eo,
        UDSliceCoordinatePhase1 ud1,
        CornerPermutationCoordinate cp,
        EdgePermutationCoordinate ep,
        UDSliceCoordinatePhase2 ud2)
    {
        var phase1Moves = _phase1Solver.Solve(co.Coordinate, eo.Coordinate, ud1.Coordinate);
        foreach (var move in phase1Moves)
        {
            ((ICoordinate)cp).Apply(move);
            ((ICoordinate)ep).Apply(move);
            ((ICoordinate)ud2).Apply(move);
        }

        var phase2Moves = _phase2Solver.Solve(cp.Coordinate, ep.Coordinate, ud2.Coordinate);
        if (phase1Moves.Length == 0 || phase2Moves.Length == 0)
        {
            return [.. phase1Moves, .. phase2Moves];
        }

        // TODO: also check for second and second to last moves?
        // could it be so that we have
        // [phase 1] ... U D | U D ... [phase 2]?
        var lastPhase1Move = phase1Moves[^1];
        var firstPhase2Move = phase2Moves[0];
        if (lastPhase1Move.Face != firstPhase2Move.Face)
        {
            return [.. phase1Moves, .. phase2Moves];
        }

        var count = (lastPhase1Move.Count + firstPhase2Move.Count) % 4;
        if (count == 0)
        {
            return [.. phase1Moves.SkipLast(1), .. phase2Moves.Skip(1)];
        }

        return [.. phase1Moves.SkipLast(1), new(lastPhase1Move.Face, count), .. phase2Moves.Skip(1)];
    }
}