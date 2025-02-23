namespace CubeTrainer.Cube;

public sealed class MoveSequence(List<Move> moves)
{
    private static readonly List<Move> AllMoves = [
        Move.R,
        Move.R2,
        Move.RPrime,
        Move.U,
        Move.U2,
        Move.UPrime,
        Move.F,
        Move.F2,
        Move.FPrime,
        Move.L,
        Move.L2,
        Move.LPrime,
        Move.D,
        Move.D2,
        Move.DPrime,
        Move.B,
        Move.B2,
        Move.BPrime,
    ];
    private readonly List<Move> _moves = moves;

    public IReadOnlyList<Move> Moves => _moves;

    public static MoveSequence Random(int length)
    {
        var moves = new List<Move>();
        var lastMove = default(Move);
        var preLastMove = default(Move);
        for (var i = 0; i < length; i++)
        {
            Move move;
            do
            {
                var moveIndex = System.Random.Shared.Next(AllMoves.Count);
                move = AllMoves[moveIndex];
            } while (
                (lastMove is not null && move.Face == lastMove.Face) ||
                (preLastMove is not null && move.Face == preLastMove.Face && move.IsOppositeFaceTo(lastMove!)));
            preLastMove = lastMove;
            lastMove = move;
            moves.Add(move);
        }

        return new(moves);
    }

    public static MoveSequence FromString(string value)
    {
        var moves = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new([.. moves.Select(Move.FromString)]);
    }

    public override string ToString()
    {
        return string.Join(' ', _moves.Select(static m => m.ToString()));
    }

    public MoveSequence Inverse()
    {
        return new([.. _moves.Select(static m => m.Inverse()).Reverse()]);
    }

    public MoveSequence Append(MoveSequence other)
    {
        return new([.. _moves, .. other._moves]);
    }
}