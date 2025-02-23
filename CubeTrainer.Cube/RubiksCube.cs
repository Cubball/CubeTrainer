using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;
using UDSliceCoordinatePhase1 = CubeTrainer.Cube.Kociemba.Phase1.Coordinates.UDSliceCoordinate;
using UDSliceCoordinatePhase2 = CubeTrainer.Cube.Kociemba.Phase2.Coordinates.UDSliceCoordinate;

namespace CubeTrainer.Cube;

public sealed class RubiksCube
{
    private RubiksCube()
    {
        CornerOrientationCoordinate = new(0);
        EdgeOrientationCoordinate = new(0);
        UDSliceCoordinatePhase1 = new(0);
        CornerPermutationCoordinate = new(0);
        EdgePermutationCoordinate = new(0);
        UDSliceCoordinatePhase2 = new(0);
    }

    internal CornerOrientationCoordinate CornerOrientationCoordinate { get; }

    internal EdgeOrientationCoordinate EdgeOrientationCoordinate { get; }

    internal UDSliceCoordinatePhase1 UDSliceCoordinatePhase1 { get; }

    internal CornerPermutationCoordinate CornerPermutationCoordinate { get; }

    internal EdgePermutationCoordinate EdgePermutationCoordinate { get; }

    internal UDSliceCoordinatePhase2 UDSliceCoordinatePhase2 { get; }

    public static RubiksCube Solved()
    {
        return new();
    }

    public static RubiksCube Scrambled(MoveSequence scramble)
    {
        var cube = new RubiksCube();
        cube.Apply(scramble);
        return cube;
    }

    public void Apply(MoveSequence moveSequence)
    {
        foreach (var move in moveSequence.Moves)
        {
            Apply(move);
        }
    }

    public void Apply(Move move)
    {
        var moveModel = new Kociemba.Common.Models.Move(move.Face, move.Count);
        ((ICoordinate)CornerOrientationCoordinate).Apply(moveModel);
        ((ICoordinate)EdgeOrientationCoordinate).Apply(moveModel);
        ((ICoordinate)UDSliceCoordinatePhase1).Apply(moveModel);
        ((ICoordinate)CornerPermutationCoordinate).Apply(moveModel);
        ((ICoordinate)EdgePermutationCoordinate).Apply(moveModel);
        ((ICoordinate)UDSliceCoordinatePhase2).Apply(moveModel);
    }
}