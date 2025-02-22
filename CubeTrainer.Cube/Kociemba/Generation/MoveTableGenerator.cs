using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Common.Tables;

namespace CubeTrainer.Cube.Kociemba.Generation;

internal static class MoveTableGenerator
{
    public static MoveTable<TCoord> Generate<TCoord>()
        where TCoord : ICoordinate
    {
        var moveTable = new MoveTable<TCoord>();
        var possibleMoves = TCoord.PossibleMoves;
        for (ushort coord = 0; coord < TCoord.PossibleCoordinatesCount; coord++)
        {
            foreach (var move in possibleMoves)
            {
                var coordinate = TCoord.Create(coord);
                coordinate.Apply(move);
                moveTable.SetValue(coord, move, coordinate.Coordinate);
            }
        }

        return moveTable;
    }
}