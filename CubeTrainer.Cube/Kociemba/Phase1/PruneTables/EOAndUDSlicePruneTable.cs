using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

internal class EOAndUDSlicePruneTable
{
    private readonly byte[] _moves;

    public EOAndUDSlicePruneTable()
    {
        _moves = new byte[EntriesCount];
    }

    public EOAndUDSlicePruneTable(byte[] moves)
    {
        _moves = moves;
    }

    public static int EntriesCount => EdgeOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount;

    public Span<byte> Moves => _moves;

    public byte GetMoves(int eoCoordinate, int udCoordinate)
    {
        return Moves[GetIndex(eoCoordinate, udCoordinate)];
    }

    public void SetMoves(int eoCoordinate, int udCoordinate, byte moves)
    {
        Moves[GetIndex(eoCoordinate, udCoordinate)] = moves;
    }

    private static int GetIndex(int eoCoordinate, int udCoordinate)
    {
        // NOTE: using constants instead of static property for efficiency
        return (eoCoordinate * Constants.UDSlicePossibleCoordinatesCount) + udCoordinate;
    }
}