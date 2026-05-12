namespace CubeTrainer.Cube;

public sealed record Move
{
    internal Move(char face, int count)
    {
        ThrowIfFaceIsInvalid(face);
        if (count is < 1 or > 3)
        {
            throw new ArgumentException("The count should be between 1 and 3", nameof(count));
        }

        Face = face;
        Count = count;
    }

    public char Face { get; }

    public int Count { get; }

    public bool IsRotation => Face is 'x' or 'y' or 'z';

    public bool IsSliceMove => Face is 'S' or 'M' or 'E';

    public bool IsWideMove => !IsRotation && Face == char.ToLowerInvariant(Face);

    public static void ThrowIfFaceIsInvalid(char face)
    {
        if (face is 'S' or 'M' or 'E' or 'x' or 'y' or 'z')
        {
            return;
        }

        face = char.ToUpperInvariant(face);
        if (face is not 'R' and not 'U' and not 'F' and not 'L' and not 'D' and not 'B')
        {
            throw new ArgumentException("The face should be one of the following: R, U, F, L, D, B, S, M, E, r, u, f, l, d, b", nameof(face));
        }
    }

    public static Move FromString(string value)
    {
        value = value.Trim();
        if (value.Length is < 1 or > 2)
        {
            throw new ArgumentException("The string should contain 1 or 2 non-whitespace characters: face, and the direction of the move (optionally) ", nameof(value));
        }

        var count = 1;
        var face = value[0];
        ThrowIfFaceIsInvalid(face);
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

        return new(face, count);
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

    public Move? Add(Move other)
    {
        if (Face != other.Face)
        {
            throw new InvalidOperationException("The moves should have the same face");
        }

        var count = (Count + other.Count) % 4;
        return count == 0 ? null : new(Face, count);
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
