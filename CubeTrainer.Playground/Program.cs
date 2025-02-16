using CubeTrainer.Cube.Kociemba.Generation;
using CubeTrainer.Cube.Kociemba.Phase1.Coordinates;
using CubeTrainer.Cube.Kociemba.Phase1.Generation;

var coMoveTable = MoveTableGenerator.Generate<CornerOrientationCoordinate, ushort>();
var eoMoveTable = MoveTableGenerator.Generate<EdgeOrientationCoordinate, ushort>();
var udMoveTable = MoveTableGenerator.Generate<UDSliceCoordinate, ushort>();
EOAndUDSlicePruneTableGenerator.GenerateToFile(eoMoveTable, udMoveTable, "D:\\EOAndUDSlicePruneTable");