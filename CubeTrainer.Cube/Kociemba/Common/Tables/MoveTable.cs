namespace CubeTrainer.Cube.Kociemba.Common.Tables;

internal class MoveTable<T> where T : ICoordinate
{
    private readonly ushort[] _buffer;
    private readonly List<Move> _possibleMoves;

    public MoveTable()
    {
        _possibleMoves = T.PossibleMoves;
        var size = T.PossibleCoordinatesCount * _possibleMoves.Count;
        _buffer = new ushort[size];
    }

    public MoveTable(ushort[] buffer)
    {
        _possibleMoves = T.PossibleMoves;
        _buffer = buffer;
    }

    public Span<ushort> Buffer => _buffer;

    public ushort GetValue(int coordinate, Move move)
    {
        return _buffer[GetBufferIndex(coordinate, move)];
    }

    public void SetValue(int coordinate, Move move, ushort value)
    {
        _buffer[GetBufferIndex(coordinate, move)] = value;
    }

    private int GetBufferIndex(int coordinate, Move move)
    {
        // PERF: use dictionary?
        var moveIdx = _possibleMoves.IndexOf(move);
        return (coordinate * _possibleMoves.Count) + moveIdx;
    }
}