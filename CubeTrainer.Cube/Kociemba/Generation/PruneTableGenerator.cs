using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Tables;

namespace CubeTrainer.Cube.Kociemba.Generation;

internal static class PruneTableGenerator
{
    public static PruneTable<TFirstCoord, TSecondCoord> GenerateToFile<TFirstCoord, TSecondCoord>(
        MoveTable<TFirstCoord> firstMoveTable,
        MoveTable<TSecondCoord> secondMoveTable,
        string filePath)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        var table = GetPruneTableFromFileOrCreate<TFirstCoord, TSecondCoord>(filePath);
        var startIndex = GetStartIndex(table);
        if (startIndex == -1)
        {
            Console.WriteLine("Table is already full");
            return table;
        }

        var startFirstCoord = (ushort)(startIndex / TSecondCoord.PossibleCoordinatesCount);
        Console.WriteLine($"Starting from first coordinate = {startFirstCoord} (byte index = {startIndex})");
        var queue = new Queue<(ushort, ushort, byte)>();
        for (ushort first = startFirstCoord; first < TFirstCoord.PossibleCoordinatesCount; first++)
        {
            for (ushort second = 0; second < TSecondCoord.PossibleCoordinatesCount; second++)
            {
                var moves = BFS(first, second, firstMoveTable, secondMoveTable, queue);
                table.SetValue(first, second, moves);
                Console.WriteLine($"Finished second coordinate = {second}");
            }

            Console.WriteLine($"Finished first coordinate = {first}");
            WriteProgressToFile(table.Buffer, filePath);
        }

        WriteProgressToFile(table.Buffer, filePath);
        return table;
    }

    private static void WriteProgressToFile(Span<byte> buffer, string filePath)
    {
        File.WriteAllBytes(filePath, buffer.ToArray());
        Console.WriteLine("Written progress to file");
    }

    private static int GetStartIndex<TFirstCoord, TSecondCoord>(PruneTable<TFirstCoord, TSecondCoord> table)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        for (var i = 0; i < table.Buffer.Length; i++)
        {
            if (table.Buffer[i] == PruneTable<TFirstCoord, TSecondCoord>.EmptyEntry)
            {
                return i;
            }
        }

        return -1;
    }

    private static PruneTable<TFirstCoord, TSecondCoord> GetPruneTableFromFileOrCreate<TFirstCoord, TSecondCoord>(string filePath)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        var entriesCount = PruneTable<TFirstCoord, TSecondCoord>.EntriesCount;
        if (File.Exists(filePath))
        {
            var fileContent = File.ReadAllBytes(filePath);
            if (fileContent.Length == entriesCount)
            {
                return new PruneTable<TFirstCoord, TSecondCoord>(fileContent);
            }

            if (fileContent.Length > 0)
            {
                throw new InvalidOperationException("The length of the file is not valid");
            }
        }

        var bytes = new byte[entriesCount];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = PruneTable<TFirstCoord, TSecondCoord>.EmptyEntry;
        }

        return new PruneTable<TFirstCoord, TSecondCoord>(bytes);
    }

    private static byte BFS<TFirstCoord, TSecondCoord>(
        int startFirstCoord,
        int startSecondCoord,
        MoveTable<TFirstCoord> firstMoveTable,
        MoveTable<TSecondCoord> secondMoveTable,
        Queue<(ushort, ushort, byte)> queue)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        var possibleMoves = TFirstCoord.PossibleMoves;
        var multiplier = TSecondCoord.PossibleCoordinatesCount;
        queue.Clear();
        Span<bool> visited = stackalloc bool[TFirstCoord.PossibleCoordinatesCount * TSecondCoord.PossibleCoordinatesCount];
        visited.Clear();
        queue.Enqueue(((ushort)startFirstCoord, (ushort)startSecondCoord, 0));
        visited[(startFirstCoord * multiplier) + startSecondCoord] = true;
        while (queue.Count > 0)
        {
            var (first, second, moves) = queue.Dequeue();
            if (first == 0 && second == 0)
            {
                return moves;
            }

            foreach (var move in possibleMoves)
            {
                var nextFirst = firstMoveTable.GetValue(first, move);
                var nextSecond = secondMoveTable.GetValue(second, move);
                var index = (nextFirst * multiplier) + nextSecond;
                if (!visited[index])
                {
                    visited[index] = true;
                    queue.Enqueue((nextFirst, nextSecond, (byte)(moves + 1)));
                }
            }
        }

        Console.WriteLine($"Was not able to find a solved state ({startFirstCoord}; {startSecondCoord})");
        return PruneTable<TFirstCoord, TSecondCoord>.InvalidEntry;
    }
}