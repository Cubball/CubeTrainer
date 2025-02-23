using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

internal sealed class UDSliceCoordinate : ICoordinate
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
    private const int MaxK = 3;

    private readonly bool[] _edges = new bool[12];

    public UDSliceCoordinate(ushort coordinate)
    {
        CoordinateToEdges(coordinate);
    }

    public static ushort PossibleCoordinatesCount { get; } = 495;

    public static List<Common.Models.Move> PossibleMoves { get; } = Constants.Phase1Moves;

    public ushort Coordinate => EdgesToCoordinate();

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
        var db = _edges[DBIndex];
        var bl = _edges[BLIndex];
        var br = _edges[BRIndex];
        _edges[UBIndex] = br;
        _edges[BRIndex] = db;
        _edges[DBIndex] = bl;
        _edges[BLIndex] = ub;
    }

    private void D()
    {
        var dr = _edges[DRIndex];
        var df = _edges[DFIndex];
        var dl = _edges[DLIndex];
        var db = _edges[DBIndex];
        _edges[DFIndex] = dl;
        _edges[DLIndex] = db;
        _edges[DBIndex] = dr;
        _edges[DRIndex] = df;
    }

    private void F()
    {
        var uf = _edges[UFIndex];
        var df = _edges[DFIndex];
        var fl = _edges[FLIndex];
        var fr = _edges[FRIndex];
        _edges[UFIndex] = fl;
        _edges[FLIndex] = df;
        _edges[DFIndex] = fr;
        _edges[FRIndex] = uf;
    }

    private void L()
    {
        var ul = _edges[ULIndex];
        var dl = _edges[DLIndex];
        var fl = _edges[FLIndex];
        var bl = _edges[BLIndex];
        _edges[FLIndex] = ul;
        _edges[ULIndex] = bl;
        _edges[BLIndex] = dl;
        _edges[DLIndex] = fl;
    }

    private void R()
    {
        var ur = _edges[URIndex];
        var dr = _edges[DRIndex];
        var fr = _edges[FRIndex];
        var br = _edges[BRIndex];
        _edges[FRIndex] = dr;
        _edges[URIndex] = fr;
        _edges[BRIndex] = ur;
        _edges[DRIndex] = br;
    }

    private void U()
    {
        var ur = _edges[URIndex];
        var uf = _edges[UFIndex];
        var ul = _edges[ULIndex];
        var ub = _edges[UBIndex];
        _edges[URIndex] = ub;
        _edges[UBIndex] = ul;
        _edges[ULIndex] = uf;
        _edges[UFIndex] = ur;
    }

    private ushort EdgesToCoordinate()
    {
        var k = MaxK;
        var coordinate = 0;
        for (var idx = _edges.Length - 1; idx >= 0; idx--)
        {
            if (k < 0)
            {
                break;
            }

            if (_edges[idx])
            {
                k--;
                continue;
            }

            coordinate += Utils.NChooseK(idx, k);
        }

        return (ushort)coordinate;
    }

    private void CoordinateToEdges(ushort coordinate)
    {
        var k = MaxK;
        for (var idx = _edges.Length - 1; idx >= 0; idx--)
        {
            if (k < 0)
            {
                break;
            }

            var value = Utils.NChooseK(idx, k);
            if (value > coordinate)
            {
                _edges[idx] = true;
                k--;
                continue;
            }

            coordinate -= (ushort)value;
        }
    }
}