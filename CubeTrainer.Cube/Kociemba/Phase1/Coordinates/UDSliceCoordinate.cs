using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Coordinates;

namespace CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

internal class UDSliceCoordinate : ICoordinate<ushort>
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
    private const int MaxIndex = 11;
    private const int MaxK = 3;

    private readonly bool[] _edges = new bool[MaxIndex + 1];

    public UDSliceCoordinate(ushort coordinate)
    {
        CoordinateToEdges(coordinate);
    }

    public UDSliceCoordinate(
        bool ur,
        bool uf,
        bool ul,
        bool ub,
        bool dr,
        bool df,
        bool dl,
        bool db,
        bool fr,
        bool fl,
        bool bl,
        bool br)
    {
        _edges[URIndex] = ur;
        _edges[UFIndex] = uf;
        _edges[ULIndex] = ul;
        _edges[UBIndex] = ub;
        _edges[DRIndex] = dr;
        _edges[DFIndex] = df;
        _edges[DLIndex] = dl;
        _edges[DBIndex] = db;
        _edges[FRIndex] = fr;
        _edges[FLIndex] = fl;
        _edges[BLIndex] = bl;
        _edges[BRIndex] = br;
    }

    public static int PossibleCoordinatesCount => 495;

    public ushort Coordinate => EdgesToCoordinate();

    public static ICoordinate<ushort> Create(ushort value)
    {
        return new UDSliceCoordinate(value);
    }

    public void B(int count = 1)
    {
        throw new NotImplementedException();
    }

    public void D(int count = 1)
    {
        throw new NotImplementedException();
    }

    public void F(int count = 1)
    {
        throw new NotImplementedException();
    }

    public void L(int count = 1)
    {
        throw new NotImplementedException();
    }

    public void R(int count = 1)
    {
        throw new NotImplementedException();
    }

    public void U(int count = 1)
    {
        throw new NotImplementedException();
    }

    private ushort EdgesToCoordinate()
    {
        var k = MaxK;
        var coordinate = 0;
        for (var idx = MaxIndex; idx >= 0; idx--)
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
        for (var idx = MaxIndex; idx >= 0; idx--)
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