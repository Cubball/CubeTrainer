using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase1.Generation.MoveTables;

var moveTable = CornerOrientationMoveTable.LoadFromDisk();
var coord = new CornerOrientationCoordinate(1, 0, 1, 0, 1, 0, 0);
var next = coord.Coordinate;
Console.WriteLine(next);
next = moveTable.GetValue(next, 'R', 1);
Console.WriteLine(next);
next = moveTable.GetValue(next, 'U', 1);
Console.WriteLine(next);
next = moveTable.GetValue(next, 'R', 3);
Console.WriteLine(next);
next = moveTable.GetValue(next, 'U', 3);
Console.WriteLine(next);