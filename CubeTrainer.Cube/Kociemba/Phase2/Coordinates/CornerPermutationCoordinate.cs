using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Phase2.Coordinates;

internal class CornerPermutationCoordinate : ICoordinate
{
    private const int URFIndex = 0;
    private const int UFLIndex = 1;
    private const int ULBIndex = 2;
    private const int UBRIndex = 3;
    private const int DFRIndex = 4;
    private const int DLFIndex = 5;
    private const int DBLIndex = 6;
    private const int DRBIndex = 7;

    private readonly int[] _corners = new int[8];

    public CornerPermutationCoordinate(ushort coordinate)
    {
        CoordinateToCorners(coordinate);
    }

    public static ushort PossibleCoordinatesCount { get; } = 40_320; // 8!

    public static List<Common.Models.Move> PossibleMoves => Constants.Phase2Moves;

    public ushort Coordinate => CornersToCoordinate();

    public static ICoordinate Create(ushort value)
    {
        return new CornerPermutationCoordinate(value);
    }

    public void B(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            B();
        }
    }

    public void D(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            D();
        }
    }

    public void F(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            F();
        }
    }

    public void L(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            L();
        }
    }

    public void R(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            R();
        }
    }

    public void U(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            U();
        }
    }

    private void B()
    {
        var ubr = _corners[UBRIndex];
        var ulb = _corners[ULBIndex];
        var drb = _corners[DRBIndex];
        var dbl = _corners[DBLIndex];
        _corners[UBRIndex] = drb;
        _corners[DRBIndex] = dbl;
        _corners[DBLIndex] = ulb;
        _corners[ULBIndex] = ubr;
    }

    private void D()
    {
        var dfr = _corners[DFRIndex];
        var drb = _corners[DRBIndex];
        var dbl = _corners[DBLIndex];
        var dlf = _corners[DLFIndex];
        _corners[DFRIndex] = dlf;
        _corners[DRBIndex] = dfr;
        _corners[DBLIndex] = drb;
        _corners[DLFIndex] = dbl;
    }

    private void F()
    {
        var urf = _corners[URFIndex];
        var ufl = _corners[UFLIndex];
        var dfr = _corners[DFRIndex];
        var dlf = _corners[DLFIndex];
        _corners[URFIndex] = ufl;
        _corners[UFLIndex] = dlf;
        _corners[DLFIndex] = dfr;
        _corners[DFRIndex] = urf;
    }

    private void L()
    {
        var ufl = _corners[UFLIndex];
        var dlf = _corners[DLFIndex];
        var dbl = _corners[DBLIndex];
        var ulb = _corners[ULBIndex];
        _corners[UFLIndex] = ulb;
        _corners[DLFIndex] = ufl;
        _corners[DBLIndex] = dlf;
        _corners[ULBIndex] = dbl;
    }

    private void R()
    {
        var urf = _corners[URFIndex];
        var dfr = _corners[DFRIndex];
        var drb = _corners[DRBIndex];
        var ubr = _corners[UBRIndex];
        _corners[URFIndex] = dfr;
        _corners[DFRIndex] = drb;
        _corners[DRBIndex] = ubr;
        _corners[UBRIndex] = urf;
    }

    private void U()
    {
        var urf = _corners[URFIndex];
        var ubr = _corners[UBRIndex];
        var ulb = _corners[ULBIndex];
        var ufl = _corners[UFLIndex];
        _corners[URFIndex] = ubr;
        _corners[UBRIndex] = ulb;
        _corners[ULBIndex] = ufl;
        _corners[UFLIndex] = urf;
    }


    private ushort CornersToCoordinate()
    {
        ushort coordinate = 0;
        for (var i = 1; i < _corners.Length; i++)
        {
            var count = 0;
            for (var j = 0; j < i; j++)
            {
                if (_corners[j] > _corners[i])
                {
                    count++;
                }
            }

            coordinate += (ushort)(count * Utils.Factorial(i));
        }

        return coordinate;
    }

    private void CoordinateToCorners(ushort coordinate)
    {
        Span<int> counts = stackalloc int[_corners.Length];
        for (var i = counts.Length - 1; i > 0; i--)
        {
            var factorial = Utils.Factorial(i);
            var count = coordinate / factorial;
            coordinate -= (ushort)(count * factorial);
            counts[i] = count;
        }

        for (var i = 1; i < counts.Length; i++)
        {
            _corners[i] = i - counts[i];
            for (var j = 0; j < i; j++)
            {
                if (_corners[j] >= _corners[i])
                {
                    _corners[j]++;
                }
            }
        }
    }
}