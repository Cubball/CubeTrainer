namespace CubeTrainer.Cube;

public record Move
{
    private Move(char face, int count)
    {
        Face = face;
        Count = count;
    }

    public static Move R { get; } = new('R', 1);

    public static Move R2 { get; } = new('R', 2);

    public static Move RPrime { get; } = new('R', 3);

    public static Move U { get; } = new('U', 1);

    public static Move U2 { get; } = new('U', 2);

    public static Move UPrime { get; } = new('U', 3);

    public static Move F { get; } = new('F', 1);

    public static Move F2 { get; } = new('F', 2);

    public static Move FPrime { get; } = new('F', 3);

    public static Move L { get; } = new('L', 1);

    public static Move L2 { get; } = new('L', 2);

    public static Move LPrime { get; } = new('L', 3);

    public static Move D { get; } = new('D', 1);

    public static Move D2 { get; } = new('D', 2);

    public static Move DPrime { get; } = new('D', 3);

    public static Move B { get; } = new('B', 1);

    public static Move B2 { get; } = new('B', 2);

    public static Move BPrime { get; } = new('B', 3);

    public char Face { get; }

    public int Count { get; }

    public static Move FromString(string value)
    {
        value = value.Trim();
        if (value.Length is < 1 or > 2)
        {
            throw new ArgumentException("The string should contain 1 or 2 non-whitespace characters: face, and the direction of the move (optionally) ", nameof(value));
        }

        var count = 1;
        var face = value[0];
        if (value.Length == 2)
        {
            var direction = value[1];
            count = direction switch
            {
                '2' => 2,
                '\'' => 3,
                _ => throw new ArgumentException("The direction should be either \"2\" or \"'\"", nameof(value)),
            };
        }

        return face is 'R' or 'U' or 'F' or 'L' or 'D' or 'B'
            ? new(face, count)
            : throw new ArgumentException("The face should be one of the following: R, U, F, L, D, B", nameof(value));
    }

    public override string ToString()
    {
        return Count == 1 ? Face.ToString() : Count == 2 ? $"{Face}2" : $"{Face}'";
    }

    public Move Inverse()
    {
        var count = 4 - Count;
        return new(Face, count);
    }

    public bool IsOppositeFaceTo(Move other)
    {
        return (Face == 'U' && other.Face == 'D')
            || (Face == 'D' && other.Face == 'U')
            || (Face == 'R' && other.Face == 'L')
            || (Face == 'L' && other.Face == 'R')
            || (Face == 'F' && other.Face == 'B')
            || (Face == 'B' && other.Face == 'F');
    }
}