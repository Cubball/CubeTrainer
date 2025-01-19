using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1.Generation.MoveTables;

// TODO: internal
public static class CornerOrientationMoveTable
{
    private const int PossibleCoordinatesCount = 2187; // 3^7
    private const int PossibleMoveDirectionsCount = 3;
    private const string Path = "D:\\MoveTable";
    private static readonly char[] Moves = ['U', 'F', 'R', 'L', 'D', 'B'];

    public static void GenerateAndWriteToDisk()
    {
        var moveTable = new MoveTable<ushort>(PossibleCoordinatesCount);
        for (ushort coord = 1; coord < PossibleCoordinatesCount; coord++)
        {
            foreach (var move in Moves)
            {
                for (var count = 1; count <= PossibleMoveDirectionsCount; count++)
                {
                    var coordinate = new CornerOrientationCoordinate(coord);
                    switch (move)
                    {
                        case 'U':
                            coordinate.U(count);
                            break;
                        case 'F':
                            coordinate.F(count);
                            break;
                        case 'R':
                            coordinate.R(count);
                            break;
                        case 'L':
                            coordinate.L(count);
                            break;
                        case 'D':
                            coordinate.D(count);
                            break;
                        case 'B':
                            coordinate.B(count);
                            break;
                        default:
                            break;
                    }

                    moveTable.SetValue(coord, move, count, coordinate.Coordinate);
                }
            }
        }

        WriteToDisk(moveTable.Buffer);
    }

    public static MoveTable<ushort> LoadFromDisk()
    {
        const int ushortSize = sizeof(ushort);
        var byteBuffer = File.ReadAllBytes(Path);
        if (byteBuffer.Length % ushortSize != 0)
        {
            throw new InvalidDataException($"The file size is not a multiple of {ushortSize}. It may be corrupted or not contain valid ushort data.");
        }

        var ushortArray = new ushort[byteBuffer.Length / ushortSize];
        for (int i = 0; i < ushortArray.Length; i++)
        {
            ushortArray[i] = BitConverter.ToUInt16(byteBuffer, i * 2);
        }

        return new MoveTable<ushort>(ushortArray);
    }

    private static void WriteToDisk(Span<ushort> buffer)
    {
        using var fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write);
        Span<byte> byteBuffer = stackalloc byte[buffer.Length * sizeof(ushort)];
        for (int i = 0; i < buffer.Length; i++)
        {
            BitConverter.TryWriteBytes(byteBuffer.Slice(i * 2, 2), buffer[i]);
        }

        fileStream.Write(byteBuffer);
    }
}