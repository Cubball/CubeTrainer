namespace CubeTrainer.Cube.Kociemba.Common.Coordinates;

internal interface ICoordinate<T>
{
    static abstract ICoordinate<T> Create(T value);

    static abstract int PossibleCoordinatesCount { get; }

    T Coordinate { get; }

    void R(int count = 1);

    void U(int count = 1);

    void F(int count = 1);

    void L(int count = 1);

    void D(int count = 1);

    void B(int count = 1);
}