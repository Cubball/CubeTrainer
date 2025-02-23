using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Common.Tables;

internal sealed class MoveTable<T> where T : ICoordinate
{
    private readonly ushort[] _buffer;
    private readonly List<Models.Move> _possibleMoves;

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

    public ushort GetValue(int coordinate, Models.Move move)
    {
        return _buffer[GetBufferIndex(coordinate, move)];
    }

    public void SetValue(int coordinate, Models.Move move, ushort value)
    {
        _buffer[GetBufferIndex(coordinate, move)] = value;
    }

    private int GetBufferIndex(int coordinate, Models.Move move)
    {
        // PERF: use dictionary?
        var moveIdx = _possibleMoves.IndexOf(move);
        return (coordinate * _possibleMoves.Count) + moveIdx;
    }
}