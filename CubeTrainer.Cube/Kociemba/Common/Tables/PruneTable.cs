using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Common.Tables;

internal sealed class PruneTable<TFirstCoord, TSecondCoord>
    where TFirstCoord : ICoordinate
    where TSecondCoord : ICoordinate
{
    public const byte EmptyEntry = byte.MaxValue;
    public const byte InvalidEntry = byte.MaxValue - 1;

    private readonly byte[] _buffer;
    private readonly ushort _secondCoordPossibleCoordinatesCount;

    public PruneTable()
    {
        _secondCoordPossibleCoordinatesCount = TSecondCoord.PossibleCoordinatesCount;
        _buffer = new byte[EntriesCount];
    }

    public PruneTable(byte[] buffer)
    {
        _secondCoordPossibleCoordinatesCount = TSecondCoord.PossibleCoordinatesCount;
        _buffer = buffer;
    }

    public static int EntriesCount => TFirstCoord.PossibleCoordinatesCount * TSecondCoord.PossibleCoordinatesCount;

    public Span<byte> Buffer => _buffer;

    public byte GetValue(ushort firstCoord, ushort secondCoord)
    {
        return _buffer[GetIndex(firstCoord, secondCoord)];
    }

    public void SetValue(ushort firstCoord, ushort secondCoord, byte value)
    {
        _buffer[GetIndex(firstCoord, secondCoord)] = value;
    }

    private int GetIndex(ushort firstCoord, ushort secondCoord)
    {
        return (firstCoord * _secondCoordPossibleCoordinatesCount) + secondCoord;
    }
}