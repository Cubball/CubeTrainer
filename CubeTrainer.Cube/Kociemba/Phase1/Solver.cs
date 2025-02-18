using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

namespace CubeTrainer.Cube.Kociemba.Phase1;

internal static class Solver
{
    private static readonly char[] Faces = ['U', 'U', 'U', 'R', 'R', 'R', 'F', 'F', 'F', 'D', 'D', 'D', 'L', 'L', 'L', 'B', 'B', 'B'];
    private static readonly char[] OppositeFaces = ['D', 'D', 'D', 'L', 'L', 'L', 'B', 'B', 'B', 'U', 'U', 'U', 'R', 'R', 'R', 'F', 'F', 'F'];
    private static readonly int[] Counts = [1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, 3];

    public static string Solve(
        int co,
        int eo,
        int udSlice,
        MoveTable<ushort> coMoveTable,
        MoveTable<ushort> eoMoveTable,
        MoveTable<ushort> udSliceMoveTable,
        COAndUDSlicePruneTable coPruneTable,
        EOAndUDSlicePruneTable eoPruneTable)
    {
        return IDA(co, eo, udSlice, coMoveTable, eoMoveTable, udSliceMoveTable, coPruneTable, eoPruneTable, 12);
    }

    private static string IDA(
        int co,
        int eo,
        int udSlice,
        MoveTable<ushort> coMoveTable,
        MoveTable<ushort> eoMoveTable,
        MoveTable<ushort> udSliceMoveTable,
        COAndUDSlicePruneTable coPruneTable,
        EOAndUDSlicePruneTable eoPruneTable,
        int maxDepth)
    {
        var estimate = Math.Max(
            coPruneTable.GetMoves(co, udSlice),
            eoPruneTable.GetMoves(eo, udSlice)
        );
        for (var depth = estimate; depth <= maxDepth; depth++)
        {
            var moves = new int[depth];
            if (IDA(co, eo, udSlice, coMoveTable, eoMoveTable, udSliceMoveTable, coPruneTable, eoPruneTable, 0, depth, moves))
            {
                var result = "";
                for (var i = 0; i < moves.Length; i++)
                {
                    result += Faces[moves[i]];
                    result += Counts[moves[i]] == 3 ? "' " : Counts[moves[i]] == 1 ? " " : "2 ";
                }

                return result;
            }

            // Console.WriteLine("done with " + depth);
        }

        return "";
    }

    private static bool IDA(
        int co,
        int eo,
        int udSlice,
        MoveTable<ushort> coMoveTable,
        MoveTable<ushort> eoMoveTable,
        MoveTable<ushort> udSliceMoveTable,
        COAndUDSlicePruneTable coPruneTable,
        EOAndUDSlicePruneTable eoPruneTable,
        int depth,
        int maxDepth,
        int[] moves)
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
            coPruneTable.GetMoves(co, udSlice),
            eoPruneTable.GetMoves(eo, udSlice)
        );
        if (maxDepth - depth < estimate)
        {
            return false;
        }

        for (var moveIdx = 0; moveIdx < 18; moveIdx++)
        {
            if (depth >= 1)
            {
                var lastMoveIdx = moves[depth - 1];
                if (Faces[moveIdx] == Faces[lastMoveIdx])
                {
                    continue;
                }

                if (depth >= 2)
                {
                    var preLastMoveIdx = moves[depth - 2];
                    var face = Faces[moveIdx];
                    var lastFace = Faces[lastMoveIdx];
                    var preLastFace = Faces[preLastMoveIdx];
                    if (lastFace == OppositeFaces[preLastMoveIdx] && preLastFace == face)
                    {
                        continue;
                    }
                }
            }

            var currentFace = Faces[moveIdx];
            var count = Counts[moveIdx];
            moves[depth] = moveIdx;
            if (IDA(
                    coMoveTable.GetValue(co, currentFace, count),
                    eoMoveTable.GetValue(eo, currentFace, count),
                    udSliceMoveTable.GetValue(udSlice, currentFace, count),
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