using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

namespace CubeTrainer.Cube.Kociemba.Phase1.Generation;

internal static class EOAndUDSlicePruneTableGenerator
{
    public static EOAndUDSlicePruneTable GenerateToFile(
        MoveTable<ushort> eoMoveTable,
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

        var startEo = startIndex / Constants.UDSlicePossibleCoordinatesCount;
        Console.WriteLine($"Starting from EO = {startEo} (byte index = {startIndex})");
        var queue = new Queue<(ushort, ushort, byte)>();
        for (var eo = startEo; eo < Constants.EdgeOrientationPossibleCoordinatesCount; eo++)
        {
            for (var ud = 0; ud < Constants.UDSlicePossibleCoordinatesCount; ud++)
            {
                var moves = BFS(eo, ud, eoMoveTable, udMoveTable, queue);
                table.SetMoves(eo, ud, moves);
            }

            Console.WriteLine($"Finished EO = {eo}");
            WriteProgressToFile(table, filePath);
        }

        WriteProgressToFile(table, filePath);
        return table;
    }

    private static void WriteProgressToFile(EOAndUDSlicePruneTable table, string filePath)
    {
        File.WriteAllBytes(filePath, table.Moves.ToArray());
        Console.WriteLine("Written progress to file");
    }

    private static int GetStartIndex(EOAndUDSlicePruneTable table)
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

    private static EOAndUDSlicePruneTable GetPruneTableFromFileOrCreate(string filePath)
    {
        if (File.Exists(filePath))
        {
            var fileContent = File.ReadAllBytes(filePath);
            if (fileContent.Length == EOAndUDSlicePruneTable.EntriesCount)
            {
                return new EOAndUDSlicePruneTable(fileContent);
            }

            if (fileContent.Length > 0)
            {
                throw new InvalidOperationException("The length of the file is not valid");
            }
        }

        var bytes = new byte[EOAndUDSlicePruneTable.EntriesCount];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Constants.PruneTableEmptyEntry;
        }

        return new EOAndUDSlicePruneTable(bytes);
    }

    private static byte BFS(
        int startEo,
        int startUd,
        MoveTable<ushort> eoMoveTable,
        MoveTable<ushort> udMoveTable,
        Queue<(ushort, ushort, byte)> queue)
    {
        queue.Clear();
        Span<bool> visited = stackalloc bool[EdgeOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount];
        visited.Clear();
        queue.Enqueue(((ushort)startEo, (ushort)startUd, 0));
        visited[(startEo * Constants.UDSlicePossibleCoordinatesCount) + startUd] = true;
        while (queue.Count > 0)
        {
            var (eo, ud, moves) = queue.Dequeue();
            if (eo == 0 && ud == 0)
            {
                return moves;
            }

            // oh god
            var (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 1), udMoveTable.GetValue(ud, 'R', 1));
            var index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 2), udMoveTable.GetValue(ud, 'R', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'R', 3), udMoveTable.GetValue(ud, 'R', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 1), udMoveTable.GetValue(ud, 'L', 1));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 2), udMoveTable.GetValue(ud, 'L', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'L', 3), udMoveTable.GetValue(ud, 'L', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 1), udMoveTable.GetValue(ud, 'U', 1));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 2), udMoveTable.GetValue(ud, 'U', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'U', 3), udMoveTable.GetValue(ud, 'U', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 1), udMoveTable.GetValue(ud, 'D', 1));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 2), udMoveTable.GetValue(ud, 'D', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'D', 3), udMoveTable.GetValue(ud, 'D', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 1), udMoveTable.GetValue(ud, 'F', 1));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 2), udMoveTable.GetValue(ud, 'F', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'F', 3), udMoveTable.GetValue(ud, 'F', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 1), udMoveTable.GetValue(ud, 'B', 1));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 2), udMoveTable.GetValue(ud, 'B', 2));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }

            (nextEo, nextUd) = (eoMoveTable.GetValue(eo, 'B', 3), udMoveTable.GetValue(ud, 'B', 3));
            index = (nextEo * Constants.UDSlicePossibleCoordinatesCount) + nextUd;
            if (!visited[index])
            {
                visited[index] = true;
                queue.Enqueue((nextEo, nextUd, (byte)(moves + 1)));
            }
        }

        Console.WriteLine($"Was not able to find a solved state ({startEo}; {startUd})");
        return Constants.PruneTableNotFoundEntry;
    }
}