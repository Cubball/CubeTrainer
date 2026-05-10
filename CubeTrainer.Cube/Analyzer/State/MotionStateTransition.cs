internal sealed record MotionStateTransition(Motion Motion) : IStateTransition
{
    public AnalyzerState? Apply(AnalyzerState state)
    {
        var motionHandOffset = (int)state.RightHandOffset;
        if (Motion.Hand == Hand.Left)
        {
            motionHandOffset = (int)state.LeftHandOffset;
        }

        var offsetDelta = GetHandOffsetDelta();
        var newOffset = motionHandOffset + offsetDelta;
        if (newOffset is > 2 or < -2)
        {
            return null;
        }

        return Motion.Hand == Hand.Left
            ? (state with
            {
                LastNonWristMotion = Motion,
                MovesCompleted = state.MovesCompleted + 1,
                LeftHandOffset = (HandOffset)newOffset,
            })
            : (state with
            {
                LastNonWristMotion = Motion,
                MovesCompleted = state.MovesCompleted + 1,
                RightHandOffset = (HandOffset)newOffset,
            });
    }

    public double GetCost(AnalyzerState state)
    {
        return Motion.Hand == Hand.Left
            ? GetCost(state.LeftHandOffset, state.LastNonWristMotion)
            : GetCost(state.RightHandOffset, state.LastNonWristMotion);
    }

    private double GetCost(HandOffset handOffset, Motion? lastNonWristMotion)
    {
        var penalty = 0.0;
        var multiplier = 1.0;
        if (handOffset == HandOffset.ThumbOnD || handOffset == HandOffset.ThumbOnU)
        {
            multiplier = CostConfig.OneFromHomeGripMultiplier;
        }
        else if (handOffset == HandOffset.FlippedTop || handOffset == HandOffset.FlippedBottom)
        {
            multiplier = CostConfig.FlippedRegripCost;
        }

        if (lastNonWristMotion is not null && lastNonWristMotion.Hand == Motion.Hand && lastNonWristMotion.Type.IsSameTypeAs(Motion.Type))
        {
            penalty = CostConfig.OverworkingPenalty;
        }

        return GetRawMotionCost() * multiplier + penalty;
    }

    private double GetRawMotionCost()
    {
        return Motion.Type switch
        {
            MotionType.WristUp or
            MotionType.WristDown or
            MotionType.DoubleWristUp or
            MotionType.DoubleWristUp => CostConfig.WristTurnCost,
            MotionType.TripleWristUp or
            MotionType.TripleWristDown => CostConfig.TripleWristTurnCost,
            MotionType.IndexPull => CostConfig.IndexPullCost,
            MotionType.IndexPush => CostConfig.IndexPushCost,
            MotionType.DoubleIndexPull => CostConfig.DoubleIndexPullCost,
            MotionType.ThumbPull => CostConfig.ThumbPullCost,
            MotionType.ThumbPush => CostConfig.ThumbPushCost,
            MotionType.RingPull => CostConfig.RingPullCost,
            MotionType.RingPush => CostConfig.RingPushCost,
            MotionType.DoubleRingPull => CostConfig.DoubleRingPullCost,
            MotionType.MiddlePull => CostConfig.MiddlePullCost,
            MotionType.MiddlePush => CostConfig.MiddlePushCost,
            _ => 0.0,
        };
    }

    private int GetHandOffsetDelta()
    {
        return Motion.Type switch
        {
            MotionType.WristUp => 1,
            MotionType.WristDown => -1,
            MotionType.DoubleWristUp => 2,
            MotionType.DoubleWristDown => -2,
            MotionType.TripleWristUp => 3,
            MotionType.TripleWristDown => -3,
            _ => 0,
        };
    }
}
