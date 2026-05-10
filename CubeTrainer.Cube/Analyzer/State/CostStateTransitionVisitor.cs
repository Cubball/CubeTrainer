using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal class CostStateTransitionVisitor : IStateTransitionVisitor
{
    public double LastCost { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        var motion = motionStateTransition.Motion;
        var state = motionStateTransition.CurrentState;
        LastCost = motion.Hand == Hand.Left
            ? GetMotionCost(motion, state.LeftHandOffset, state.LastNonWristMotion)
            : GetMotionCost(motion, state.RightHandOffset, state.LastNonWristMotion);
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        var regrip = regripStateTransition.Regrip;
        LastCost = regrip.NewHandOffset == HandOffset.FlippedBottom || regrip.NewHandOffset == HandOffset.FlippedTop
            ? CostConfig.FlippedRegripCost
            : CostConfig.RegripCost;
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        LastCost = CostConfig.RotationCost;
    }

    private static double GetMotionCost(Motion motion, HandOffset currentHandOffset, Motion? lastNonWristMotion)
    {
        // TODO: account for slice moves?
        var penalty = 0.0;
        var multiplier = 1.0;
        if (currentHandOffset == HandOffset.ThumbOnD || currentHandOffset == HandOffset.ThumbOnU)
        {
            multiplier = CostConfig.OneFromHomeGripMultiplier;
        }
        else if (currentHandOffset == HandOffset.FlippedTop || currentHandOffset == HandOffset.FlippedBottom)
        {
            multiplier = CostConfig.FlippedRegripCost;
        }

        if (lastNonWristMotion is not null && lastNonWristMotion.Hand == motion.Hand && lastNonWristMotion.Type.IsSameTypeAs(motion.Type))
        {
            penalty = CostConfig.OverworkingPenalty;
        }

        return GetRawMotionCost(motion.Type) * multiplier + penalty;
    }

    private static double GetRawMotionCost(MotionType motionType)
    {
        return motionType switch
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
}
