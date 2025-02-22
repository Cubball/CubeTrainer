using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase2;

internal class Phase2Solver(
    MoveTable<CornerPermutationCoordinate> cpMoveTable,
    MoveTable<EdgePermutationCoordinate> epMoveTable,
    MoveTable<UDSliceCoordinate> udSliceMoveTable,
    PruneTable<CornerPermutationCoordinate, UDSliceCoordinate> cpPruneTable,
    PruneTable<EdgePermutationCoordinate, UDSliceCoordinate> epPruneTable)
{
    private const int MaxDepth = 18;

    private readonly MoveTable<CornerPermutationCoordinate> _cpMoveTable = cpMoveTable;
    private readonly MoveTable<EdgePermutationCoordinate> _epMoveTable = epMoveTable;
    private readonly MoveTable<UDSliceCoordinate> _udSliceMoveTable = udSliceMoveTable;
    private readonly PruneTable<CornerPermutationCoordinate, UDSliceCoordinate> _cpPruneTable = cpPruneTable;
    private readonly PruneTable<EdgePermutationCoordinate, UDSliceCoordinate> _epPruneTable = epPruneTable;

    public Move[] Solve(
        ushort cp,
        ushort ep,
        ushort udSlice)
    {
        return IDA(cp, ep, udSlice, MaxDepth);
    }

    private Move[] IDA(
        ushort cp,
        ushort ep,
        ushort udSlice,
        int maxDepth)
    {
        var minMovesToSolve = Math.Max(
            _cpPruneTable.GetValue(cp, udSlice),
            _epPruneTable.GetValue(ep, udSlice)
        );
        for (var depth = minMovesToSolve; depth <= maxDepth; depth++)
        {
            var moves = new Move[depth];
            if (IDA(cp, ep, udSlice, 0, depth, moves))
            {
                return moves;
            }
        }

        return [];
    }

    private bool IDA(
        ushort cp,
        ushort ep,
        ushort udSlice,
        int depth,
        int maxDepth,
        Move[] moves)
    {
        if (cp == 0 && ep == 0 && udSlice == 0)
        {
            return true;
        }

        if (depth >= maxDepth)
        {
            return false;
        }

        var minMovesToSolve = Math.Max(
            _cpPruneTable.GetValue(cp, udSlice),
            _epPruneTable.GetValue(ep, udSlice)
        );
        if (maxDepth - depth < minMovesToSolve)
        {
            return false;
        }

        foreach (var move in Constants.Phase2Moves)
        {
            if (depth >= 1)
            {
                var lastMove = moves[depth - 1];
                if (move.Face == lastMove.Face)
                {
                    continue;
                }

                if (depth >= 2)
                {
                    var preLastMove = moves[depth - 2];
                    if (Utils.AreOppositeFaces(lastMove.Face, preLastMove.Face) && preLastMove.Face == move.Face)
                    {
                        continue;
                    }
                }
            }

            moves[depth] = move;
            if (IDA(
                    _cpMoveTable.GetValue(cp, move),
                    _epMoveTable.GetValue(ep, move),
                    _udSliceMoveTable.GetValue(udSlice, move),
                    depth + 1,
                    maxDepth,
                    moves))
            {
                return true;
            }
        }

        return false;
    }
}