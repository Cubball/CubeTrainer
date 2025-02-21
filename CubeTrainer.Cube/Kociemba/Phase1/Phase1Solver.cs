using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1;

internal class Phase1Solver(
    MoveTable<CornerOrientationCoordinate> coMoveTable,
    MoveTable<EdgeOrientationCoordinate> eoMoveTable,
    MoveTable<UDSliceCoordinate> udSliceMoveTable,
    PruneTable<CornerOrientationCoordinate, UDSliceCoordinate> coPruneTable,
    PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate> eoPruneTable)
{
    private const int MaxDepth = 12;

    private readonly MoveTable<CornerOrientationCoordinate> _coMoveTable = coMoveTable;
    private readonly MoveTable<EdgeOrientationCoordinate> _eoMoveTable = eoMoveTable;
    private readonly MoveTable<UDSliceCoordinate> _udSliceMoveTable = udSliceMoveTable;
    private readonly PruneTable<CornerOrientationCoordinate, UDSliceCoordinate> _coPruneTable = coPruneTable;
    private readonly PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate> _eoPruneTable = eoPruneTable;

    public Move[] Solve(
        ushort co,
        ushort eo,
        ushort udSlice)
    {
        return IDA(co, eo, udSlice, MaxDepth);
    }

    private Move[] IDA(
        ushort co,
        ushort eo,
        ushort udSlice,
        int maxDepth)
    {
        var minMovesToSolve = Math.Max(
            _coPruneTable.GetValue(co, udSlice),
            _eoPruneTable.GetValue(eo, udSlice)
        );
        for (var depth = minMovesToSolve; depth <= maxDepth; depth++)
        {
            var moves = new Move[depth];
            if (IDA(co, eo, udSlice, 0, depth, moves))
            {
                return moves;
            }
        }

        return [];
    }

    private bool IDA(
        ushort co,
        ushort eo,
        ushort udSlice,
        int depth,
        int maxDepth,
        Move[] moves)
    {
        if (co == 0 && eo == 0 && udSlice == 0)
        {
            return true;
        }

        if (depth >= maxDepth)
        {
            return false;
        }

        var minMovesToSolve = Math.Max(
            _coPruneTable.GetValue(co, udSlice),
            _eoPruneTable.GetValue(eo, udSlice)
        );
        if (maxDepth - depth < minMovesToSolve)
        {
            return false;
        }

        foreach (var move in Constants.Phase1Moves)
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
                    _coMoveTable.GetValue(co, move),
                    _eoMoveTable.GetValue(eo, move),
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