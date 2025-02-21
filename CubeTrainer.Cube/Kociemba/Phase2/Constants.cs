using CubeTrainer.Cube.Kociemba.Common;

namespace CubeTrainer.Cube.Kociemba.Phase2;

internal static class Constants
{
    public static readonly List<Move> Phase2Moves = [
        new('U', 1),
        new('U', 2),
        new('U', 3),
        new('R', 2),
        new('F', 2),
        new('D', 1),
        new('D', 2),
        new('D', 3),
        new('L', 2),
        new('B', 2),
    ];
}