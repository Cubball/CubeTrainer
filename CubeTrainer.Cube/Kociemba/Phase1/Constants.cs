namespace CubeTrainer.Cube.Kociemba.Phase1;

internal class Constants
{
    public const int UDSlicePossibleCoordinatesCount = 495;

    public const int EdgeOrientationPossibleCoordinatesCount = 2048;

    public const byte PruneTableEmptyEntry = byte.MaxValue;

    public const byte PruneTableNotFoundEntry = byte.MaxValue - 1;
}