using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Common.Tables;

namespace CubeTrainer.Cube.Kociemba.Infrastructure;

internal static class FileManager
{
    private static readonly string GeneratedBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Kociemba", "Generated");

    public static readonly string COMoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "co");
    public static readonly string EOMoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "eo");
    public static readonly string UDSlicePhase1MoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "ud1");
    public static readonly string CPMoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "cp");
    public static readonly string EPMoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "ep");
    public static readonly string UDSlicePhase2MoveTablePath = Path.Combine(GeneratedBaseDirectory, "MoveTables", "ud2");
    public static readonly string COPruneTablePath = Path.Combine(GeneratedBaseDirectory, "PruneTables", "co_ud");
    public static readonly string EOPruneTablePath = Path.Combine(GeneratedBaseDirectory, "PruneTables", "eo_ud");
    public static readonly string CPPruneTablePath = Path.Combine(GeneratedBaseDirectory, "PruneTables", "cp_ud");
    public static readonly string EPPruneTablePath = Path.Combine(GeneratedBaseDirectory, "PruneTables", "ep_ud");

    public static MoveTable<T> LoadMoveTableFromFile<T>(string filePath) where T : ICoordinate
    {
        var bytes = File.ReadAllBytes(filePath);
        if (bytes.Length % 2 != 0)
        {
            throw new InvalidOperationException("The file containing the move table should have an even number of bytes");
        }

        var buffer = new ushort[bytes.Length / 2];
        for (var i = 0; i < buffer.Length; i++)
        {
            ushort entry = 0;
            entry |= bytes[2 * i];
            entry |= (ushort)(bytes[(2 * i) + 1] << 4);
            buffer[i] = entry;
        }

        return new(buffer);
    }

    public static void WriteMoveTableToFile<T>(string filePath, MoveTable<T> moveTable) where T : ICoordinate
    {
        var bytes = new byte[moveTable.Buffer.Length * 2];
        for (var i = 0; i < moveTable.Buffer.Length; i++)
        {
            bytes[2 * i] = (byte)(moveTable.Buffer[i] & 0b1111);
            bytes[(2 * i) + 1] = (byte)(moveTable.Buffer[i] >>> 4);
        }

        File.WriteAllBytes(filePath, bytes);
    }

    public static PruneTable<TFirstCoord, TSecondCoord> LoadPruneTableFromFile<TFirstCoord, TSecondCoord>(string filePath)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        var bytes = File.ReadAllBytes(filePath);
        return new(bytes);
    }

    public static void WritePruneTableToFile<TFirstCoord, TSecondCoord>(string filePath, PruneTable<TFirstCoord, TSecondCoord> pruneTable)
        where TFirstCoord : ICoordinate
        where TSecondCoord : ICoordinate
    {
        var bytes = pruneTable.Buffer;
        File.WriteAllBytes(filePath, bytes.ToArray());
    }
}