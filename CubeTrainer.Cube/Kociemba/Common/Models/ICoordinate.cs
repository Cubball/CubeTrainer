namespace CubeTrainer.Cube.Kociemba.Common.Models;

internal interface ICoordinate
{
    static abstract ICoordinate Create(ushort value);

    static abstract ushort PossibleCoordinatesCount { get; }

    static abstract List<Move> PossibleMoves { get; }

    ushort Coordinate { get; }

    void U(int count = 1);

    void R(int count = 1);

    void F(int count = 1);

    void D(int count = 1);

    void L(int count = 1);

    void B(int count = 1);

    void Apply(Move move)
    {
        switch (move.Face)
        {
            case 'U':
                U(move.Count);
                break;
            case 'R':
                R(move.Count);
                break;
            case 'F':
                F(move.Count);
                break;
            case 'D':
                D(move.Count);
                break;
            case 'L':
                L(move.Count);
                break;
            case 'B':
                B(move.Count);
                break;
            default:
                // NOTE: not supporting wide moves, rotaions or slice moves
                // since the Kociemba is used to generate random scrambles.
                // Scrambles usually don't include these moves and we can
                // use a default solution for each case that does not include
                // these moves as well.
                break;
        }
    }
}