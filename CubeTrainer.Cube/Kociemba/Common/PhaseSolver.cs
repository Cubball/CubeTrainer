using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Common.Tables;

namespace CubeTrainer.Cube.Kociemba.Common;

internal class PhaseSolver<TFirstCoord, TSecondCoord, TThirdCoord>(
    List<Move> availableMoves,
    int maxDepth,
    MoveTable<TFirstCoord> firstMoveTable,
    MoveTable<TSecondCoord> secondMoveTable,
    MoveTable<TThirdCoord> thirdMoveTable,
    PruneTable<TFirstCoord, TThirdCoord> firstThirdPruneTable,
    PruneTable<TSecondCoord, TThirdCoord> secondThirdPruneTable)
    where TFirstCoord : ICoordinate
    where TSecondCoord : ICoordinate
    where TThirdCoord : ICoordinate
{
    private readonly List<Move> _availableMoves = availableMoves;
    private readonly int _maxDepth = maxDepth;
    private readonly MoveTable<TFirstCoord> _firstMoveTable = firstMoveTable;
    private readonly MoveTable<TSecondCoord> _secondMoveTable = secondMoveTable;
    private readonly MoveTable<TThirdCoord> _thirdMoveTable = thirdMoveTable;
    private readonly PruneTable<TFirstCoord, TThirdCoord> _firstThirdPruneTable = firstThirdPruneTable;
    private readonly PruneTable<TSecondCoord, TThirdCoord> _secondThirdPruneTable = secondThirdPruneTable;

    public List<Move> Solve(
        ushort firstCoord,
        ushort secondCoord,
        ushort thirdCoord)
    {
        var minMovesToSolve = Math.Max(
            _firstThirdPruneTable.GetValue(firstCoord, thirdCoord),
            _secondThirdPruneTable.GetValue(secondCoord, thirdCoord)
        );
        for (var depth = minMovesToSolve; depth <= _maxDepth; depth++)
        {
            var moves = new Move[depth];
            if (IDA(firstCoord, secondCoord, thirdCoord, 0, depth, moves))
            {
                return [.. moves];
            }
        }

        return [];
    }

    private bool IDA(
        ushort firstCoord,
        ushort secondCoord,
        ushort thirdCoord,
        int depth,
        int maxDepth,
        Move[] moves)
    {
        if (firstCoord == 0 && secondCoord == 0 && thirdCoord == 0)
        {
            return true;
        }

        if (depth >= maxDepth)
        {
            return false;
        }

        var minMovesToSolve = Math.Max(
            _firstThirdPruneTable.GetValue(firstCoord, thirdCoord),
            _secondThirdPruneTable.GetValue(secondCoord, thirdCoord)
        );
        if (maxDepth - depth < minMovesToSolve)
        {
            return false;
        }

        foreach (var move in _availableMoves)
        {
            if (depth >= 1)
            {
                var lastMove = moves[depth - 1];
                if (move.Face == lastMove.Face)
                {
                    continue;
                }

                if (depth >= 2)
                {
                    var preLastMove = moves[depth - 2];
                    if (Utils.AreOppositeFaces(lastMove.Face, preLastMove.Face) && preLastMove.Face == move.Face)
                    {
                        continue;
                    }
                }
            }

            moves[depth] = move;
            if (IDA(
                    _firstMoveTable.GetValue(firstCoord, move),
                    _secondMoveTable.GetValue(secondCoord, move),
                    _thirdMoveTable.GetValue(thirdCoord, move),
                    depth + 1,
                    maxDepth,
                    moves))
            {
                return true;
            }
        }

        return false;
    }
}