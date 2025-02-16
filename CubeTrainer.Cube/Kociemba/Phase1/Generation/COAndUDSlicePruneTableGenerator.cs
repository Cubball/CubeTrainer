using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

namespace CubeTrainer.Cube.Kociemba.Phase1.Generation;

internal static class COAndUDSlicePruneTableGenerator
{
    public static COAndUDSlicePruneTable GenerateToFile(
        MoveTable<ushort> coMoveTable,
        MoveTable<ushort> udMoveTable,
        string filePath)
    {
        var table = GetPruneTableFromFileOrCreate(filePath);
        var startIndex = GetStartIndex(table);
        if (startIndex == -1)
        {
            Console.WriteLine("Table is already full");
            return table;
        }

        var startCo = startIndex / Constants.UDSlicePossibleCoordinatesCount;
        Console.WriteLine($"Starting from CO = {startCo} (byte index = {startIndex})");
        var queue = new Queue<(ushort, ushort, byte)>();
        for (var co = startCo; co < Constants.EdgeOrientationPossibleCoordinatesCount; co++)
        {
            for (var ud = 0; ud < Constants.UDSlicePossibleCoordinatesCount; ud++)
            {
                var moves = BFS(co, ud, coMoveTable, udMoveTable, queue);
                table.SetMoves(co, ud, moves);
            }

            Console.WriteLine($"Finished CO = {co}");
            WriteProgressToFile(table, filePath);
        }

        WriteProgressToFile(table, filePath);
        return table;
    }

    private static void WriteProgressToFile(COAndUDSlicePruneTable table, string filePath)
    {
        File.WriteAllBytes(filePath, table.Moves.ToArray());
        Console.WriteLine("Written progress to file");
    }

    private static int GetStartIndex(COAndUDSlicePruneTable table)
    {
        for (var i = 0; i < table.Moves.Length; i++)
        {
            if (table.Moves[i] == Constants.PruneTableEmptyEntry)
            {
                return i;
            }
        }

        return -1;
    }

    private static COAndUDSlicePruneTable GetPruneTableFromFileOrCreate(string filePath)
    {
        if (File.Exists(filePath))
        {
            var fileContent = File.ReadAllBytes(filePath);
            if (fileContent.Length == COAndUDSlicePruneTable.EntriesCount)
            {
                return new COAndUDSlicePruneTable(fileContent);
            }

            if (fileContent.Length > 0)
            {
                throw new InvalidOperationException("The length of the file is not valid");
            }
        }

        var bytes = new byte[COAndUDSlicePruneTable.EntriesCount];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Constants.PruneTableEmptyEntry;
        }

        return new COAndUDSlicePruneTable(bytes);
    }

    private static byte BFS(
        int startCo,
        int startUd,
        MoveTable<ushort> coMoveTable,
        MoveTable<ushort> udMoveTable,
        Queue<(ushort, ushort, byte)> queue)
    {
        queue.Clear();
        Span<bool> visited = stackalloc bool[CornerOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount];
        visited.Clear();
        queue.Enqueue(((ushort)startCo, (ushort)startUd, 0));
        visited[(startCo * Constants.UDSlicePossibleCoordinatesCount) + startUd] = true;
        while (queue.Count > 0)
        {
            var (co, ud, moves) = queue.Dequeue();
            if (co == 0 && ud == 0)
            {
                return moves;
            }

            // oh god
            var (nextCo, nextUd) = (coMoveTable.GetValue(co, 'R', 1), udMoveTable.GetValue(ud, 'R', 1));
            var index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'R', 2), udMoveTable.GetValue(ud, 'R', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'R', 3), udMoveTable.GetValue(ud, 'R', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'L', 1), udMoveTable.GetValue(ud, 'L', 1));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'L', 2), udMoveTable.GetValue(ud, 'L', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'L', 3), udMoveTable.GetValue(ud, 'L', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'U', 1), udMoveTable.GetValue(ud, 'U', 1));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'U', 2), udMoveTable.GetValue(ud, 'U', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'U', 3), udMoveTable.GetValue(ud, 'U', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'D', 1), udMoveTable.GetValue(ud, 'D', 1));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'D', 2), udMoveTable.GetValue(ud, 'D', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'D', 3), udMoveTable.GetValue(ud, 'D', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'F', 1), udMoveTable.GetValue(ud, 'F', 1));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'F', 2), udMoveTable.GetValue(ud, 'F', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'F', 3), udMoveTable.GetValue(ud, 'F', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'B', 1), udMoveTable.GetValue(ud, 'B', 1));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'B', 2), udMoveTable.GetValue(ud, 'B', 2));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }

            (nextCo, nextUd) = (coMoveTable.GetValue(co, 'B', 3), udMoveTable.GetValue(ud, 'B', 3));
            index = (nextCo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextCo, nextUd, (byte)(moves + 1)));
            }
        }

        Console.WriteLine($"Was not able to find a solved state ({startCo}; {startUd})");
        return Constants.PruneTableNotFoundEntry;
    }
}