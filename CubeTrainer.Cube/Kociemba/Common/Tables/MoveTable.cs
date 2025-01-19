namespace CubeTrainer.Cube.Kociemba.Common.Tables;

// TODO: internal
public class MoveTable<T>
{
    private const int PossibleMoveDirectionsCount = 3;
    private const int FacesCount = 6;
    private const int PossibleMovesCount = PossibleMoveDirectionsCount * FacesCount;

    private static readonly Dictionary<char, int> MoveIndexes = new()
    {
        { 'U', 0 },
        { 'F', 1 },
        { 'R', 2 },
        { 'L', 3 },
        { 'D', 4 },
        { 'B', 5 },
    };

    private readonly T[] _buffer;

    public MoveTable(int possibleCoordinatesCount)
    {
        var size = possibleCoordinatesCount * PossibleMovesCount;
        _buffer = new T[size];
    }

    public MoveTable(T[] buffer)
    {
        _buffer = buffer;
    }

    public Span<T> Buffer => _buffer;

    public T GetValue(int coordinate, char move, int count)
    {
        return _buffer[GetBufferIndex(coordinate, move, count)];
    }

    public void SetValue(int coordinate, char move, int count, T value)
    {
        _buffer[GetBufferIndex(coordinate, move, count)] = value;
    }

    private static int GetBufferIndex(int coordinate, char move, int count)
    {
        var moveIdx = MoveIndexes[move];
        // - 1 at the end since the count is in [1; 3], but buffer offset should be in [0; 2]
        return (coordinate * PossibleMovesCount) + (moveIdx * PossibleMoveDirectionsCount) + count - 1;
    }
}