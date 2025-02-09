using System.Numerics;
using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Coordinates;
using CubeTrainer.Cube.Kociemba.Common.Tables;

namespace CubeTrainer.Cube.Kociemba.Generation;

internal static class MoveTableGenerator
{
    public static MoveTable<TData> Generate<TCoord, TData>()
        where TCoord : ICoordinate<TData>
        where TData : INumberBase<TData>
    {
        var moveTable = new MoveTable<TData>(TCoord.PossibleCoordinatesCount);
        for (var coord = 1; coord < TCoord.PossibleCoordinatesCount; coord++)
        {
            // NOTE: phase 2 would include less moves, or at least not all quater moves.
            // Might add an enum with moves like R', U2, and method to ICoordinate that would accept it
            // then there would be something like a MoveProvider?
            foreach (var move in Constants.Moves)
            {
                for (var count = 1; count <= Constants.PossibleMoveDirectionsCount; count++)
                {
                    var coordinate = TCoord.Create(TData.CreateChecked(coord));
                    switch (move)
                    {
                        case 'U':
                            coordinate.U(count);
                            break;
                        case 'F':
                            coordinate.F(count);
                            break;
                        case 'R':
                            coordinate.R(count);
                            break;
                        case 'L':
                            coordinate.L(count);
                            break;
                        case 'D':
                            coordinate.D(count);
                            break;
                        case 'B':
                            coordinate.B(count);
                            break;
                        default:
                            break;
                    }

                    moveTable.SetValue(coord, move, count, coordinate.Coordinate);
                }
            }
        }

        return moveTable;
    }
}