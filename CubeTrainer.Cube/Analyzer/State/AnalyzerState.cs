using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal sealed record AnalyzerState(
    int MovesCompleted,
    HandOffset LeftHandOffset,
    HandOffset RightHandOffset,
    Motion? LastNonWristMotion,
    Hand? LastHandRegrip
);
