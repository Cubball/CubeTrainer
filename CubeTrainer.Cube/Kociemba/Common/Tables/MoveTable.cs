namespace CubeTrainer.Cube.Kociemba.Common.Tables;

internal class MoveTable<T>
{
    private readonly T[] _buffer;

    public MoveTable(int possibleCoordinatesCount)
    {
        var size = possibleCoordinatesCount * Constants.PossibleMovesCount;
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
        var moveIdx = Constants.MoveIndexes[move];
        // - 1 at the end since the count is in [1; 3], but buffer offset should be in [0; 2]
        return (coordinate * Constants.PossibleMovesCount) + (moveIdx * Constants.PossibleMoveDirectionsCount) + count - 1;
    }
}