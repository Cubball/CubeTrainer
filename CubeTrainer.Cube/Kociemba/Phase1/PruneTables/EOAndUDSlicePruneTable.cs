using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

internal class EOAndUDSlicePruneTable
{
    private const int UDSlicePossibleCoordinatesCount = 495; // redefined here as a constant for efficiency

    private static readonly Queue<(ushort, ushort, byte)> Queue = new();

    // FIXME:
    public readonly byte[] _moves = new byte[EdgeOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount];

    private EOAndUDSlicePruneTable() { }

    public static EOAndUDSlicePruneTable Generate(
        MoveTable<ushort> eoMoveTable,
        MoveTable<ushort> udMoveTable)
    {
        var table = new EOAndUDSlicePruneTable();
        for (var i = 0; i < table._moves.Length; i++)
        {
            table._moves[i] = byte.MaxValue;
        }

        for (var eo = 0; eo < EdgeOrientationCoordinate.PossibleCoordinatesCount; eo++)
        {
            for (var ud = 0; ud < UDSliceCoordinate.PossibleCoordinatesCount; ud++)
            {
                var moves = BFS(eo, ud, eoMoveTable, udMoveTable);
                table.SetMoves(eo, ud, moves);
            }

            Console.WriteLine($"Finished {eo}");
        }

        return table;
    }

    private static byte BFS(int startEo, int startUd, MoveTable<ushort> eoMoveTable, MoveTable<ushort> udMoveTable)
    {
        Queue.Clear();
        Span<bool> visited = stackalloc bool[EdgeOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount];
        Queue.Enqueue(((ushort)startEo, (ushort)startUd, 0));
        visited[GetIndex(startEo, startUd)] = true;
        while (Queue.Count > 0)
        {
            var (eo, ud, moves) = Queue.Dequeue();
            if (eo == 0 && ud == 0)
            {
                return moves;
            }

            // oh god
            var (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 1), udMoveTable.GetValue(ud, 'R', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 2), udMoveTable.GetValue(ud, 'R', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 3), udMoveTable.GetValue(ud, 'R', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 1), udMoveTable.GetValue(ud, 'L', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 2), udMoveTable.GetValue(ud, 'L', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 3), udMoveTable.GetValue(ud, 'L', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 1), udMoveTable.GetValue(ud, 'U', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 2), udMoveTable.GetValue(ud, 'U', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 3), udMoveTable.GetValue(ud, 'U', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 1), udMoveTable.GetValue(ud, 'D', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 2), udMoveTable.GetValue(ud, 'D', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 3), udMoveTable.GetValue(ud, 'D', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 1), udMoveTable.GetValue(ud, 'F', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 2), udMoveTable.GetValue(ud, 'F', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 3), udMoveTable.GetValue(ud, 'F', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 1), udMoveTable.GetValue(ud, 'B', 1));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 2), udMoveTable.GetValue(ud, 'B', 2));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 3), udMoveTable.GetValue(ud, 'B', 3));
            if (!visited[GetIndex(nextEo, nextUd)])
            {
                visited[GetIndex(nextEo, nextUd)] = true;
                Queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }
        }

        throw new Exception("Was not able to find a solved state");
    }

    public byte GetMoves(int eoCoordinate, int udCoordinate)
    {
        return _moves[GetIndex(eoCoordinate, udCoordinate)];
    }

    private void SetMoves(int eoCoordinate, int udCoordinate, byte moves)
    {
        _moves[GetIndex(eoCoordinate, udCoordinate)] = moves;
    }

    private static int GetIndex(int eoCoordinate, int udCoordinate)
    {
        return (eoCoordinate * UDSlicePossibleCoordinatesCount) + udCoordinate;
    }
}