namespace CubeTrainer.Cube.Analyzer;

internal static class CostConfig
{
    public static double RotationCost { get; } = 4.0;

    public static double RegripCost { get; } = 1.0;

    public static double FlippedRegripCost { get; } = 2.0;

    public static double OverworkingPenalty { get; } = 0.5;

    public static double OneFromHomeGripMultiplier { get; } = 1.5;

    public static double TwoFromHomeGripMultiplier { get; } = 3.0;

    public static double WristTurnCost { get; } = 1.0;

    public static double TripleWristTurnCost { get; } = 2.0; // 1.0 to favor R3 in U perm

    public static double IndexPullCost { get; } = 1.0;

    public static double DoubleIndexPullCost { get; } = 1.5;

    public static double IndexPushCost { get; } = 1.5;

    public static double ThumbPushCost { get; } = 2.5;

    public static double ThumbPullCost { get; } = 2.5;

    public static double DoubleThumbPullCost { get; } = 5.0;

    public static double RingPullCost { get; } = 2.0;

    public static double RingPushCost { get; } = 2.0;

    public static double DoubleRingPullCost { get; } = 2.5;

    public static double MiddlePullCost { get; } = 2.0;

    public static double MiddlePushCost { get; } = 2.5;

    public static double DoubleMiddlePullCost { get; } = 4.0;

    public static double SliceMoveMultiplier { get; } = 1.5;
}
