namespace CubeTrainer.Cube.Kociemba.Common;

internal class Constants
{
    public const int PossibleMoveDirectionsCount = 3;
    public const int FacesCount = 6;
    public const int PossibleMovesCount = PossibleMoveDirectionsCount * FacesCount;

    public static readonly char[] Moves = ['U', 'R', 'F', 'D', 'L', 'B'];
    public static readonly Dictionary<char, int> MoveIndexes = new()
    {
        { 'U', 0 },
        { 'R', 1 },
        { 'F', 2 },
        { 'D', 3 },
        { 'L', 4 },
        { 'B', 5 },
    };
}