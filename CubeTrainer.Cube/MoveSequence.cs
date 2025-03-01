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
    private readonly List<Move> _moves = Normalize(moves);

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

    private static List<Move> Normalize(List<Move> moves)
    {
        var normalizedMoves = new List<Move>();
        for (var i = 0; i < moves.Count; i++)
        {
            var sameFaceMove = default(Move);
            var oppositeFaceMove = default(Move);
            var j = i;
            while (j < moves.Count)
            {
                if (moves[i].Face == moves[j].Face)
                {
                    sameFaceMove = sameFaceMove is null ? moves[j] : sameFaceMove.Add(moves[j]);
                }
                else if (moves[i].IsOppositeFaceTo(moves[j]))
                {
                    oppositeFaceMove = oppositeFaceMove is null ? moves[j] : oppositeFaceMove.Add(moves[j]);
                }
                else
                {
                    break;
                }

                j++;
            }

            if (sameFaceMove is not null)
            {
                normalizedMoves.Add(sameFaceMove);
            }

            if (oppositeFaceMove is not null)
            {
                normalizedMoves.Add(oppositeFaceMove);
            }

            i = j - 1;
        }

        // HACK:
        // if the number of moves and normalized moves is the same
        // that means that the moves are already normalized;
        // we do a recursive call for cases like "R U U' R'"
        // after the first normalization it'll get transformed
        // to "R R'", after second it'll be no moves;
        // the proper way to do it would be to implement an
        // algorithm similar to checking whether the parentheses
        // are valid, "R U U' R'" could be seen as "[{}]"
        return moves.Count == normalizedMoves.Count
            ? normalizedMoves
            : Normalize(normalizedMoves);
    }
}