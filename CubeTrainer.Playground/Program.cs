using System.Diagnostics;
using CubeTrainer.Cube.Kociemba;
using CubeTrainer.Cube.Kociemba.Common;
using CubeTrainer.Cube.Kociemba.Common.Models;
using CubeTrainer.Cube.Kociemba.Infrastructure;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase2.Coordinates;
using UDSliceCoordinatePhase1 = CubeTrainer.Cube.Kociemba.Phase1.Coordinates.UDSliceCoordinate;
using UDSliceCoordinatePhase2 = CubeTrainer.Cube.Kociemba.Phase2.Coordinates.UDSliceCoordinate;

var coMoveTable = FileManager.LoadMoveTableFromFile<CornerOrientationCoordinate>(FileManager.COMoveTablePath);
var eoMoveTable = FileManager.LoadMoveTableFromFile<EdgeOrientationCoordinate>(FileManager.EOMoveTablePath);
var ud1MoveTable = FileManager.LoadMoveTableFromFile<UDSliceCoordinatePhase1>(FileManager.UDSlicePhase1MoveTablePath);
var coPruneTable = FileManager.LoadPruneTableFromFile<CornerOrientationCoordinate, UDSliceCoordinatePhase1>(FileManager.COPruneTablePath);
var eoPruneTable = FileManager.LoadPruneTableFromFile<EdgeOrientationCoordinate, UDSliceCoordinatePhase1>(FileManager.EOPruneTablePath);

var cpMoveTable = FileManager.LoadMoveTableFromFile<CornerPermutationCoordinate>(FileManager.CPMoveTablePath);
var epMoveTable = FileManager.LoadMoveTableFromFile<EdgePermutationCoordinate>(FileManager.EPMoveTablePath);
var ud2MoveTable = FileManager.LoadMoveTableFromFile<UDSliceCoordinatePhase2>(FileManager.UDSlicePhase2MoveTablePath);
var cpPruneTable = FileManager.LoadPruneTableFromFile<CornerPermutationCoordinate, UDSliceCoordinatePhase2>(FileManager.CPPruneTablePath);
var epPruneTable = FileManager.LoadPruneTableFromFile<EdgePermutationCoordinate, UDSliceCoordinatePhase2>(FileManager.EPPruneTablePath);

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