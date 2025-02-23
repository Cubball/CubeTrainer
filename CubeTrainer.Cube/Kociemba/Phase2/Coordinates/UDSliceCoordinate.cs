using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Phase2.Coordinates;

internal class UDSliceCoordinate : ICoordinate
{
    private const int URIndex = 0;
    private const int UFIndex = 1;
    private const int ULIndex = 2;
    private const int UBIndex = 3;
    private const int DRIndex = 4;
    private const int DFIndex = 5;
    private const int DLIndex = 6;
    private const int DBIndex = 7;
    private const int FRIndex = 8;
    private const int FLIndex = 9;
    private const int BLIndex = 10;
    private const int BRIndex = 11;

    private readonly int[] _edges = new int[12];

    public UDSliceCoordinate(ushort coordinate)
    {
        CoordinateToCorners(coordinate);
    }

    public static ushort PossibleCoordinatesCount { get; } = 24; // 4!

    public static List<Common.Models.Move> PossibleMoves => Constants.Phase2Moves;

    public ushort Coordinate => CornersToCoordinate();

    public static ICoordinate Create(ushort value)
    {
        return new UDSliceCoordinate(value);
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
        var ub = _edges[UBIndex];
        var br = _edges[BRIndex];
        var db = _edges[DBIndex];
        var bl = _edges[BLIndex];
        _edges[UBIndex] = br;
        _edges[BRIndex] = db;
        _edges[DBIndex] = bl;
        _edges[BLIndex] = ub;
    }

    private void D()
    {
        var df = _edges[DFIndex];
        var dl = _edges[DLIndex];
        var db = _edges[DBIndex];
        var dr = _edges[DRIndex];
        _edges[DFIndex] = dl;
        _edges[DLIndex] = db;
        _edges[DBIndex] = dr;
        _edges[DRIndex] = df;
    }

    private void F()
    {
        var uf = _edges[UFIndex];
        var fr = _edges[FRIndex];
        var df = _edges[DFIndex];
        var fl = _edges[FLIndex];
        _edges[UFIndex] = fl;
        _edges[FRIndex] = uf;
        _edges[DFIndex] = fr;
        _edges[FLIndex] = df;
    }

    private void L()
    {
        var fl = _edges[FLIndex];
        var ul = _edges[ULIndex];
        var bl = _edges[BLIndex];
        var dl = _edges[DLIndex];
        _edges[FLIndex] = ul;
        _edges[ULIndex] = bl;
        _edges[BLIndex] = dl;
        _edges[DLIndex] = fl;
    }

    private void R()
    {
        var fr = _edges[FRIndex];
        var ur = _edges[URIndex];
        var br = _edges[BRIndex];
        var dr = _edges[DRIndex];
        _edges[FRIndex] = dr;
        _edges[URIndex] = fr;
        _edges[BRIndex] = ur;
        _edges[DRIndex] = br;
    }

    private void U()
    {
        var uf = _edges[UFIndex];
        var ul = _edges[ULIndex];
        var ub = _edges[UBIndex];
        var ur = _edges[URIndex];
        _edges[UFIndex] = ur;
        _edges[ULIndex] = uf;
        _edges[UBIndex] = ul;
        _edges[URIndex] = ub;
    }


    private ushort CornersToCoordinate()
    {
        ushort coordinate = 0;
        // we only use edges in the equator slice
        for (var i = FLIndex; i <= BRIndex; i++)
        {
            var count = 0;
            for (var j = FRIndex; j < i; j++)
            {
                if (_edges[j] > _edges[i])
                {
                    count++;
                }
            }

            coordinate += (ushort)(count * Utils.Factorial(i - FRIndex));
        }

        return coordinate;
    }

    private void CoordinateToCorners(ushort coordinate)
    {
        Span<int> counts = stackalloc int[BRIndex - FRIndex + 1];
        for (var i = counts.Length - 1; i > 0; i--)
        {
            var factorial = Utils.Factorial(i);
            var count = coordinate / factorial;
            coordinate -= (ushort)(count * factorial);
            counts[i] = count;
        }

        _edges[FRIndex] = FRIndex;
        for (var i = 1; i < counts.Length; i++)
        {
            _edges[i + FRIndex] = i - counts[i] + FRIndex;
            for (var j = 0; j < i; j++)
            {
                if (_edges[j + FRIndex] >= _edges[i + FRIndex])
                {
                    _edges[j + FRIndex]++;
                }
            }
        }

        for (var i = URIndex; i <= DBIndex; i++)
        {
            _edges[i] = i;
        }
    }
}