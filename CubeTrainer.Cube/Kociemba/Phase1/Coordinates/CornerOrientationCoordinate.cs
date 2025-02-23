using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;

namespace CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

internal sealed class CornerOrientationCoordinate(ushort coordinate) : ICoordinate
{
    private const ushort URFMod = 729; // 3^6
    private const ushort UFLMod = 243; // 3^5
    private const ushort ULBMod = 81;  // 3^4
    private const ushort UBRMod = 27;  // 3^3
    private const ushort DFRMod = 9;   // 3^2
    private const ushort DLFMod = 3;   // 3^1
    private const ushort DBLMod = 1;   // 3^0

    public static ushort PossibleCoordinatesCount { get; } = 2187; // 3^7

    public static List<Common.Models.Move> PossibleMoves { get; } = Constants.Phase1Moves;

    public ushort Coordinate { get; private set; } = coordinate;

    public static ICoordinate Create(ushort value)
    {
        return new CornerOrientationCoordinate(value);
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

    public void D(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            D();
        }
    }

    public void B(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            B();
        }
    }

    private void R()
    {
        var originalDFROrientation = (ushort)(Coordinate / DFRMod % 3);
        var originalURFOrientation = (ushort)(Coordinate / URFMod % 3);
        var originalUBROrientation = (ushort)(Coordinate / UBRMod % 3);
        var originalDRBOrientation = GetDRBOrientation();
        var newDFROrientation = (ushort)((originalDRBOrientation + 1) % 3);
        var newURFOrientation = (ushort)((originalDFROrientation + 2) % 3);
        var newUBROrientation = (ushort)((originalURFOrientation + 1) % 3);
        Coordinate -= (ushort)(originalDFROrientation * DFRMod);
        Coordinate -= (ushort)(originalURFOrientation * URFMod);
        Coordinate -= (ushort)(originalUBROrientation * UBRMod);
        Coordinate += (ushort)(newDFROrientation * DFRMod);
        Coordinate += (ushort)(newURFOrientation * URFMod);
        Coordinate += (ushort)(newUBROrientation * UBRMod);
    }

    private void U()
    {
        var originalURFOrientation = (ushort)(Coordinate / URFMod % 3);
        var originalUBROrientation = (ushort)(Coordinate / UBRMod % 3);
        var originalULBOrientation = (ushort)(Coordinate / ULBMod % 3);
        var originalUFLOrientation = (ushort)(Coordinate / UFLMod % 3);
        var newURFOrientation = originalUBROrientation;
        var newUFLOrientation = originalURFOrientation;
        var newULBOrientation = originalUFLOrientation;
        var newUBROrientation = originalULBOrientation;
        Coordinate -= (ushort)(originalURFOrientation * URFMod);
        Coordinate -= (ushort)(originalUFLOrientation * UFLMod);
        Coordinate -= (ushort)(originalULBOrientation * ULBMod);
        Coordinate -= (ushort)(originalUBROrientation * UBRMod);
        Coordinate += (ushort)(newURFOrientation * URFMod);
        Coordinate += (ushort)(newUFLOrientation * UFLMod);
        Coordinate += (ushort)(newULBOrientation * ULBMod);
        Coordinate += (ushort)(newUBROrientation * UBRMod);
    }

    private void F()
    {
        var originalURFOrientation = (ushort)(Coordinate / URFMod % 3);
        var originalUFLOrientation = (ushort)(Coordinate / UFLMod % 3);
        var originalDFROrientation = (ushort)(Coordinate / DFRMod % 3);
        var originalDLFOrientation = (ushort)(Coordinate / DLFMod % 3);
        var newURFOrientation = (ushort)((originalUFLOrientation + 1) % 3);
        var newUFLOrientation = (ushort)((originalDLFOrientation + 2) % 3);
        var newDLFOrientation = (ushort)((originalDFROrientation + 1) % 3);
        var newDFROrientation = (ushort)((originalURFOrientation + 2) % 3);
        Coordinate -= (ushort)(originalURFOrientation * URFMod);
        Coordinate -= (ushort)(originalUFLOrientation * UFLMod);
        Coordinate -= (ushort)(originalDFROrientation * DFRMod);
        Coordinate -= (ushort)(originalDLFOrientation * DLFMod);
        Coordinate += (ushort)(newURFOrientation * URFMod);
        Coordinate += (ushort)(newUFLOrientation * UFLMod);
        Coordinate += (ushort)(newDLFOrientation * DLFMod);
        Coordinate += (ushort)(newDFROrientation * DFRMod);
    }

    private void L()
    {
        var originalUFLOrientation = (ushort)(Coordinate / UFLMod % 3);
        var originalDLFOrientation = (ushort)(Coordinate / DLFMod % 3);
        var originalULBOrientation = (ushort)(Coordinate / ULBMod % 3);
        var originalDBLOrientation = (ushort)(Coordinate / DBLMod % 3);
        var newUFLOrientation = (ushort)((originalULBOrientation + 1) % 3);
        var newDLFOrientation = (ushort)((originalUFLOrientation + 2) % 3);
        var newDBLOrientation = (ushort)((originalDLFOrientation + 1) % 3);
        var newULBOrientation = (ushort)((originalDBLOrientation + 2) % 3);
        Coordinate -= (ushort)(originalUFLOrientation * UFLMod);
        Coordinate -= (ushort)(originalDLFOrientation * DLFMod);
        Coordinate -= (ushort)(originalULBOrientation * ULBMod);
        Coordinate -= (ushort)(originalDBLOrientation * DBLMod);
        Coordinate += (ushort)(newUFLOrientation * UFLMod);
        Coordinate += (ushort)(newDLFOrientation * DLFMod);
        Coordinate += (ushort)(newDBLOrientation * DBLMod);
        Coordinate += (ushort)(newULBOrientation * ULBMod);
    }

    private void D()
    {
        var originalDFROrientation = (ushort)(Coordinate / DFRMod % 3);
        var originalDLFOrientation = (ushort)(Coordinate / DLFMod % 3);
        var originalDBLOrientation = (ushort)(Coordinate / DBLMod % 3);
        var originalDRBOrientation = GetDRBOrientation();
        var newDFROrientation = originalDLFOrientation;
        var newDLFOrientation = originalDBLOrientation;
        var newDBLOrientation = originalDRBOrientation;
        Coordinate -= (ushort)(originalDFROrientation * DFRMod);
        Coordinate -= (ushort)(originalDLFOrientation * DLFMod);
        Coordinate -= (ushort)(originalDBLOrientation * DBLMod);
        Coordinate += (ushort)(newDFROrientation * DFRMod);
        Coordinate += (ushort)(newDLFOrientation * DLFMod);
        Coordinate += (ushort)(newDBLOrientation * DBLMod);
    }

    private void B()
    {
        var originalUBROrientation = (ushort)(Coordinate / UBRMod % 3);
        var originalULBOrientation = (ushort)(Coordinate / ULBMod % 3);
        var originalDBLOrientation = (ushort)(Coordinate / DBLMod % 3);
        var originalDRBOrientation = GetDRBOrientation();
        var newUBROrientation = (ushort)((originalDRBOrientation + 2) % 3);
        var newULBOrientation = (ushort)((originalUBROrientation + 1) % 3);
        var newDBLOrientation = (ushort)((originalULBOrientation + 2) % 3);
        Coordinate -= (ushort)(originalUBROrientation * UBRMod);
        Coordinate -= (ushort)(originalULBOrientation * ULBMod);
        Coordinate -= (ushort)(originalDBLOrientation * DBLMod);
        Coordinate += (ushort)(newUBROrientation * UBRMod);
        Coordinate += (ushort)(newULBOrientation * ULBMod);
        Coordinate += (ushort)(newDBLOrientation * DBLMod);
    }

    private byte GetDRBOrientation()
    {
        var urf = Coordinate / URFMod % 3;
        var ufl = Coordinate / UFLMod % 3;
        var ulb = Coordinate / ULBMod % 3;
        var ubr = Coordinate / UBRMod % 3;
        var dfr = Coordinate / DFRMod % 3;
        var dlf = Coordinate / DLFMod % 3;
        var dbl = Coordinate / DBLMod % 3;
        var sum = urf + ufl + ulb + ubr + dfr + dlf + dbl;
        var mod = sum % 3;
        return mod == 0
            ? (byte)0
            : (byte)(3 - mod);
    }

    private static void ThrowIfCubieOrientationInvalid(byte cubieOrientation)
    {
        if (cubieOrientation is > 2 or < 0)
        {
            throw new ArgumentException("Cubie orientation should be between 0 and 2 inclusive", nameof(cubieOrientation));
        }
    }
}