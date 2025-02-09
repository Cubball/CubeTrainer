using CubeTrainer.Cube.Kociemba.Generation;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase1.PruneTables;

var coMoveTable = MoveTableGenerator.Generate<CornerOrientationCoordinate, ushort>();
var eoMoveTable = MoveTableGenerator.Generate<EdgeOrientationCoordinate, ushort>();
var udMoveTable = MoveTableGenerator.Generate<UDSliceCoordinate, ushort>();
var pruneTable = EOAndUDSlicePruneTable.Generate(eoMoveTable, udMoveTable);

File.WriteAllBytes("D:\\EOAndUDSlicePruneTable", pruneTable._moves);