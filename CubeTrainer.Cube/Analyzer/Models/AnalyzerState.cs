internal sealed record AnalyzerState(
    int MovesCompleted,
    HandOffset LeftHandOffset,
    HandOffset RightHandOffset,
    Motion? LastMotion,
    Motion? LastNonWristMotion
);
