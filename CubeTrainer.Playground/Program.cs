using System.Diagnostics;
using CubeTrainer.Cube.Kociemba;
using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Generation;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;
using UDSliceCoordinatePhase1 = CubeTrainer.Cube.Kociemba.Phase1.Coordinates.UDSliceCoordinate;
using UDSliceCoordinatePhase2 = CubeTrainer.Cube.Kociemba.Phase2.Coordinates.UDSliceCoordinate;

var coMoveTable = MoveTableGenerator.Generate<CornerOrientationCoordinate>();
var eoMoveTable = MoveTableGenerator.Generate<EdgeOrientationCoordinate>();
var ud1MoveTable = MoveTableGenerator.Generate<UDSliceCoordinatePhase1>();
var coPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneCOAndUD");
var coPruneTable = new PruneTable<CornerOrientationCoordinate, UDSliceCoordinatePhase1>(coPruneBuffer);
var eoPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneEOAndUD");
var eoPruneTable = new PruneTable<EdgeOrientationCoordinate, UDSliceCoordinatePhase1>(eoPruneBuffer);

var cpMoveTable = MoveTableGenerator.Generate<CornerPermutationCoordinate>();
var epMoveTable = MoveTableGenerator.Generate<EdgePermutationCoordinate>();
var ud2MoveTable = MoveTableGenerator.Generate<UDSliceCoordinatePhase2>();
var cpPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneCPAndUD");
var cpPruneTable = new PruneTable<CornerPermutationCoordinate, UDSliceCoordinatePhase2>(cpPruneBuffer);
var epPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneEPAndUD");
var epPruneTable = new PruneTable<EdgePermutationCoordinate, UDSliceCoordinatePhase2>(epPruneBuffer);

var solver = new Solver(
    coMoveTable,
    eoMoveTable,
    ud1MoveTable,
    coPruneTable,
    eoPruneTable,
    cpMoveTable,
    epMoveTable,
    ud2MoveTable,
    cpPruneTable,
    epPruneTable);

// TODO: implement algorithm for random scrambles
var moves = Constants.Phase1Moves;
Move? previousMove = null;
Move? prepreviousMove = null;
var co = new CornerOrientationCoordinate(0);
var cp = new CornerPermutationCoordinate(0);
var eo = new EdgeOrientationCoordinate(0);
var ep = new EdgePermutationCoordinate(0);
var ud1 = new UDSliceCoordinatePhase1(0);
var ud2 = new UDSliceCoordinatePhase2(0);
var scramble = string.Empty;
for (var i = 0; i < 20; i++)
{
    Move move;
    do
    {
        var moveIdx = Random.Shared.Next(moves.Count);
        move = moves[moveIdx];
    } while (move.Face == previousMove?.Face || (move.Face == prepreviousMove?.Face && Utils.AreOppositeFaces(move.Face, previousMove?.Face ?? ' ')));

    prepreviousMove = previousMove;
    previousMove = move;

    ((ICoordinate)co).Apply(move);
    ((ICoordinate)cp).Apply(move);
    ((ICoordinate)eo).Apply(move);
    ((ICoordinate)ep).Apply(move);
    ((ICoordinate)ud1).Apply(move);
    ((ICoordinate)ud2).Apply(move);
    scramble += move.Face;
    scramble += move.Count == 1 ? " " : move.Count == 2 ? "2 " : "' ";
}

var stopwatch = Stopwatch.StartNew();
var solutionMoves = solver.Solve(co, eo, ud1, cp, ep, ud2);
stopwatch.Stop();
var solution = string.Empty;
foreach (var move in solutionMoves)
{
    solution += move.Face;
    solution += move.Count == 1 ? " " : move.Count == 2 ? "2 " : "' ";
}

Console.WriteLine($"Scramble: {scramble.Trim()}");
Console.WriteLine($"Solution: {solution.Trim()}");
Console.WriteLine($"Time: {stopwatch.Elapsed}");