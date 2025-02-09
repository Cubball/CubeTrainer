using CubeTrainer.Cube.Kociemba.Generation;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

var moveTable = MoveTableGenerator.Generate<CornerOrientationCoordinate, ushort>();

WriteToDisk(moveTable.Buffer);

static void WriteToDisk(Span<ushort> buffer)
{
    using var fileStream = new FileStream("D:\\MoveTableNew", FileMode.Create, FileAccess.Write);
    Span<byte> byteBuffer = stackalloc byte[buffer.Length * sizeof(ushort)];
    for (int i = 0; i < buffer.Length; i++)
    {
        BitConverter.TryWriteBytes(byteBuffer.Slice(i * 2, 2), buffer[i]);
    }

    fileStream.Write(byteBuffer);
}