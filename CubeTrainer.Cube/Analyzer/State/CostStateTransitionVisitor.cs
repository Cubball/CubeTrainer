using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal class CostStateTransitionVisitor : IStateTransitionVisitor
{
    public CostComponent? LastCost { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        var motion = motionStateTransition.Motion;
        var state = motionStateTransition.CurrentState;
        var move = motionStateTransition.Move;
        LastCost = motion.Hand == Hand.Left
            ? GetMotionCost(motion, state.LeftHandOffset, state.LastNonWristMotion, move)
            : GetMotionCost(motion, state.RightHandOffset, state.LastNonWristMotion, move);
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        var regrip = regripStateTransition.Regrip;
        LastCost = regrip.NewHandOffset == HandOffset.FlippedBottom || regrip.NewHandOffset == HandOffset.FlippedTop
            ? new CostComponent(CostConfig.FlippedRegripCost, [], [])
            : new CostComponent(CostConfig.RegripCost, [], []);
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        LastCost = new CostComponent(CostConfig.RotationCost, [], []);
    }

    private static CostComponent GetMotionCost(Motion motion, HandOffset currentHandOffset, Motion? lastNonWristMotion, Move move)
    {
        List<(double Value, string Description)> multipliers = [];
        List<(double Value, string Description)> penalties = [];
        // NOTE: maybe apply non-home grip multiplier even if it's a different hand
        // for flipped positions?
        var shouldNonHomeGripApplyMultiplier = !motion.Type.IsWristMotion();
        if (shouldNonHomeGripApplyMultiplier && (currentHandOffset == HandOffset.ThumbOnD || currentHandOffset == HandOffset.ThumbOnU))
        {
            multipliers.Add((CostConfig.OneFromHomeGripMultiplier, "one off from home grip"));
        }
        else if (shouldNonHomeGripApplyMultiplier && (currentHandOffset == HandOffset.FlippedTop || currentHandOffset == HandOffset.FlippedBottom))
        {
            multipliers.Add((CostConfig.TwoFromHomeGripMultiplier, "flipped regrip"));
        }

        if (move.IsSliceMove)
        {
            multipliers.Add((CostConfig.SliceMoveMultiplier, "slice move"));
        }

        if (lastNonWristMotion is not null && lastNonWristMotion.Hand == motion.Hand && lastNonWristMotion.Type.IsSameTypeAs(motion.Type))
        {
            penalties.Add((CostConfig.OverworkingPenalty, "overworking"));
        }

        return new CostComponent(GetRawMotionCost(motion.Type), multipliers, penalties);
    }

    private static double GetRawMotionCost(MotionType motionType)
    {
        return motionType switch
        {
            MotionType.WristUp or
            MotionType.WristDown or
            MotionType.DoubleWristUp or
            MotionType.DoubleWristDown => CostConfig.WristTurnCost,
            MotionType.TripleWristUp or
            MotionType.TripleWristDown => CostConfig.TripleWristTurnCost,
            MotionType.IndexPull => CostConfig.IndexPullCost,
            MotionType.IndexPush => CostConfig.IndexPushCost,
            MotionType.DoubleIndexPull => CostConfig.DoubleIndexPullCost,
            MotionType.ThumbPull => CostConfig.ThumbPullCost,
            MotionType.ThumbPush => CostConfig.ThumbPushCost,
            MotionType.DoubleThumbPull => CostConfig.DoubleThumbPullCost,
            MotionType.RingPull => CostConfig.RingPullCost,
            MotionType.RingPush => CostConfig.RingPushCost,
            MotionType.DoubleRingPull => CostConfig.DoubleRingPullCost,
            MotionType.MiddlePull => CostConfig.MiddlePullCost,
            MotionType.MiddlePush => CostConfig.MiddlePushCost,
            MotionType.DoubleMiddlePull => CostConfig.DoubleMiddlePullCost,
            _ => 0.0,
        };
    }
}