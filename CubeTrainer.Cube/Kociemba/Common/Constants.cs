namespace CubeTrainer.Cube.Kociemba.Common;

internal static class Constants
{
    public const int Phase1MaxDepth = 12;
    public const int Phase2MaxDepth = 18;

    public static readonly List<Models.Move> Phase1Moves = [
        new('U', 1),
        new('U', 2),
        new('U', 3),
        new('R', 1),
        new('R', 2),
        new('R', 3),
        new('F', 1),
        new('F', 2),
        new('F', 3),
        new('D', 1),
        new('D', 2),
        new('D', 3),
        new('L', 1),
        new('L', 2),
        new('L', 3),
        new('B', 1),
        new('B', 2),
        new('B', 3),
    ];
    public static readonly List<Models.Move> Phase2Moves = [
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