using System.Diagnostics;
using CubeTrainer.Cube.Kociemba.Common.Tables;
using CubeTrainer.Cube.Kociemba.Generation;
using CubeTrainer.Cube.Kociemba.Phase1;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;

var coMoveTable = MoveTableGenerator.Generate<CornerOrientationCoordinate>();
var eoMoveTable = MoveTableGenerator.Generate<EdgeOrientationCoordinate>();
var udMoveTable = MoveTableGenerator.Generate<UDSliceCoordinate>();
var coPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneCOAndUD");
var coPruneTable = new PruneTable<CornerOrientationCoordinate, UDSliceCoordinate>(coPruneBuffer);
var eoPruneBuffer = File.ReadAllBytes("D:\\Tables\\Ready\\PruneEOAndUD");
var eoPruneTable = new PruneTable<EdgeOrientationCoordinate, UDSliceCoordinate>(eoPruneBuffer);

var eo = new EdgeOrientationCoordinate(0);
var co = new CornerOrientationCoordinate(0);
var ud = new UDSliceCoordinate(0);
var scramble = "";
for (var i = 0; i < 20; i++)
{
    var count = Random.Shared.Next(1, 4);
    var face = Random.Shared.Next(0, 6);
    switch (face)
    {
        case 0:
            eo.R(count);
            co.R(count);
            ud.R(count);
            scramble += 'R';
            break;
        case 1:
            eo.U(count);
            co.U(count);
            ud.U(count);
            scramble += 'U';
            break;
        case 2:
            eo.F(count);
            co.F(count);
            ud.F(count);
            scramble += 'F';
            break;
        case 3:
            eo.L(count);
            co.L(count);
            ud.L(count);
            scramble += 'L';
            break;
        case 4:
            eo.D(count);
            co.D(count);
            ud.D(count);
            scramble += 'D';
            break;
        case 5:
            eo.B(count);
            co.B(count);
            ud.B(count);
            scramble += 'B';
            break;
        default:
            break;
    }

    scramble += count == 2 ? "2 " : count == 3 ? "' " : " ";
}

var stopwatch = Stopwatch.StartNew();
var solution = Solver.Solve(co.Coordinate, eo.Coordinate, ud.Coordinate, coMoveTable, eoMoveTable, udMoveTable, coPruneTable, eoPruneTable);
stopwatch.Stop();
Console.WriteLine(scramble);
Console.WriteLine(solution);
Console.WriteLine(stopwatch.Elapsed);