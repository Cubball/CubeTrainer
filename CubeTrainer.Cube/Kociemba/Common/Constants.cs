namespace CubeTrainer.Cube.Kociemba.Common;

internal class Constants
{
    public const int PossibleMoveDirectionsCount = 3;
    public const int FacesCount = 6;
    public const int PossibleMovesCount = PossibleMoveDirectionsCount * FacesCount;

    public static readonly char[] Moves = ['U', 'F', 'R', 'L', 'D', 'B'];
    public static readonly Dictionary<char, int> MoveIndexes = new()
    {
        { 'U', 0 },
        { 'F', 1 },
        { 'R', 2 },
        { 'L', 3 },
        { 'D', 4 },
        { 'B', 5 },
    };
}