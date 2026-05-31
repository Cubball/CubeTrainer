namespace CubeTrainer.Cube.Analyzer;

public sealed class CostConfig
{
    public static CostConfig Default { get; } = new();

    public double RotationCost { get; set; } = 4.0;

    public double RegripCost { get; set; } = 1.0;

    public double FlippedRegripCost { get; set; } = 2.0;

    public double OverworkingPenalty { get; set; } = 0.5;

    public double OneFromHomeGripMultiplier { get; set; } = 1.5;

    public double TwoFromHomeGripMultiplier { get; set; } = 3.0;

    public double WristTurnCost { get; set; } = 1.0;

    public double TripleWristTurnCost { get; set; } = 2.0;

    public double IndexPullCost { get; set; } = 1.0;

    public double DoubleIndexPullCost { get; set; } = 1.5;

    public double IndexPushCost { get; set; } = 1.5;

    public double ThumbPushCost { get; set; } = 2.5;

    public double ThumbPullCost { get; set; } = 2.5;

    public double DoubleThumbPullCost { get; set; } = 5.0;

    public double RingPullCost { get; set; } = 2.0;

    public double RingPushCost { get; set; } = 2.5;

    public double DoubleRingPullCost { get; set; } = 2.5;

    public double MiddlePullCost { get; set; } = 2.5;

    public double MiddlePushCost { get; set; } = 3.0;

    public double DoubleMiddlePullCost { get; set; } = 5.0;

    public double SliceMoveMultiplier { get; set; } = 1.5;

    public CostConfig Clone()
    {
        return new CostConfig
        {
            RotationCost = RotationCost,
            RegripCost = RegripCost,
            FlippedRegripCost = FlippedRegripCost,
            OverworkingPenalty = OverworkingPenalty,
            OneFromHomeGripMultiplier = OneFromHomeGripMultiplier,
            TwoFromHomeGripMultiplier = TwoFromHomeGripMultiplier,
            WristTurnCost = WristTurnCost,
            TripleWristTurnCost = TripleWristTurnCost,
            IndexPullCost = IndexPullCost,
            DoubleIndexPullCost = DoubleIndexPullCost,
            IndexPushCost = IndexPushCost,
            ThumbPushCost = ThumbPushCost,
            ThumbPullCost = ThumbPullCost,
            DoubleThumbPullCost = DoubleThumbPullCost,
            RingPullCost = RingPullCost,
            RingPushCost = RingPushCost,
            DoubleRingPullCost = DoubleRingPullCost,
            MiddlePullCost = MiddlePullCost,
            MiddlePushCost = MiddlePushCost,
            DoubleMiddlePullCost = DoubleMiddlePullCost,
            SliceMoveMultiplier = SliceMoveMultiplier,
        };
    }
}
