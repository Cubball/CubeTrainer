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
        using var binaryReader = new BinaryReader(File.Open(filePath, FileMode.Open));
        var values = new List<ushort>();
        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
        {
            values.Add(binaryReader.ReadUInt16());
        }

        return new([.. values]);
    }

    public static void WriteMoveTableToFile<T>(string filePath, MoveTable<T> moveTable) where T : ICoordinate
    {
        using var binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.Create));
        foreach (var value in moveTable.Buffer)
        {
            binaryWriter.Write(value);
        }
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