using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1;

internal static class Solver
{
    public static string Solve(
        ushort co,
        ushort eo,
        ushort udSlice,
        MoveTable<CornerOrientationCoordinate> coMoveTable,
        MoveTable<EdgeOrientationCoordinate> eoMoveTable,
        MoveTable<UDSliceCoordinate> udSliceMoveTable,
        PruneTable<CornerOrientationCoordinate, UDSliceCoordinate> coPruneTable,
        PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate> eoPruneTable)
    {
        return IDA(co, eo, udSlice, coMoveTable, eoMoveTable, udSliceMoveTable, coPruneTable, eoPruneTable, 12);
    }

    private static string IDA(
        ushort co,
        ushort eo,
        ushort udSlice,
        MoveTable<CornerOrientationCoordinate> coMoveTable,
        MoveTable<EdgeOrientationCoordinate> eoMoveTable,
        MoveTable<UDSliceCoordinate> udSliceMoveTable,
        PruneTable<CornerOrientationCoordinate, UDSliceCoordinate> coPruneTable,
        PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate> eoPruneTable,
        int maxDepth)
    {
        var estimate = Math.Max(
            coPruneTable.GetValue(co, udSlice),
            eoPruneTable.GetValue(eo, udSlice)
        );
        for (var depth = estimate; depth <= maxDepth; depth++)
        {
            var moves = new Move[depth];
            if (IDA(co, eo, udSlice, coMoveTable, eoMoveTable, udSliceMoveTable, coPruneTable, eoPruneTable, 0, depth, moves))
            {
                var result = "";
                for (var i = 0; i < moves.Length; i++)
                {
                    result += moves[i].Face;
                    var count = moves[i].Count;
                    result += count == 3 ? "' " : count == 1 ? " " : "2 ";
                }

                return result;
            }
        }

        return "";
    }

    private static bool IDA(
        ushort co,
        ushort eo,
        ushort udSlice,
        MoveTable<CornerOrientationCoordinate> coMoveTable,
        MoveTable<EdgeOrientationCoordinate> eoMoveTable,
        MoveTable<UDSliceCoordinate> udSliceMoveTable,
        PruneTable<CornerOrientationCoordinate, UDSliceCoordinate> coPruneTable,
        PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate> eoPruneTable,
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

        var estimate = Math.Max(
            coPruneTable.GetValue(co, udSlice),
            eoPruneTable.GetValue(eo, udSlice)
        );
        if (maxDepth - depth < estimate)
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
                    coMoveTable.GetValue(co, move),
                    eoMoveTable.GetValue(eo, move),
                    udSliceMoveTable.GetValue(udSlice, move),
                    coMoveTable,
                    eoMoveTable,
                    udSliceMoveTable,
                    coPruneTable,
                    eoPruneTable,
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