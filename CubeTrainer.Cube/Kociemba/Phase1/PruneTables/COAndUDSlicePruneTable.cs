using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

internal class COAndUDSlicePruneTable
{
    private readonly byte[] _moves;

    public COAndUDSlicePruneTable()
    {
        _moves = new byte[EntriesCount];
    }

    public COAndUDSlicePruneTable(byte[] moves)
    {
        _moves = moves;
    }

    public static int EntriesCount => CornerOrientationCoordinate.PossibleCoordinatesCount * UDSliceCoordinate.PossibleCoordinatesCount;

    public Span<byte> Moves => _moves;

    public byte GetMoves(int coCoordinate, int udCoordinate)
    {
        return Moves[GetIndex(coCoordinate, udCoordinate)];
    }

    public void SetMoves(int coCoordinate, int udCoordinate, byte moves)
    {
        Moves[GetIndex(coCoordinate, udCoordinate)] = moves;
    }

    private static int GetIndex(int coCoordinate, int udCoordinate)
    {
        // NOTE: using constants instead of static property for efficiency
        return (coCoordinate * Constants.UDSlicePossibleCoordinatesCount) + udCoordinate;
    }
}