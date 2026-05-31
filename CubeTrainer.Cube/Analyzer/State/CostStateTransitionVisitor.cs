using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal class CostStateTransitionVisitor : IStateTransitionVisitor
{
    private readonly CostConfig _costConfig;

    public CostStateTransitionVisitor(CostConfig costConfig)
    {
        _costConfig = costConfig;
    }

    public CostComponent? LastCost { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        var motion = motionStateTransition.Motion;
        var state = motionStateTransition.CurrentState;
        var move = motionStateTransition.Move;
        LastCost = motion.Hand == Hand.Left
            ? GetMotionCost(motion, state.LeftHandOffset, state.LastNonWristMotion, move, _costConfig)
            : GetMotionCost(motion, state.RightHandOffset, state.LastNonWristMotion, move, _costConfig);
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        var regrip = regripStateTransition.Regrip;
        LastCost = regrip.NewHandOffset == HandOffset.FlippedBottom || regrip.NewHandOffset == HandOffset.FlippedTop
            ? new CostComponent(_costConfig.FlippedRegripCost, [], [])
            : new CostComponent(_costConfig.RegripCost, [], []);
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        LastCost = new CostComponent(_costConfig.RotationCost, [], []);
    }

    private static CostComponent GetMotionCost(Motion motion, HandOffset currentHandOffset, Motion? lastNonWristMotion, Move move, CostConfig costConfig)
    {
        List<(double Value, string Description)> multipliers = [];
        List<(double Value, string Description)> penalties = [];
        // NOTE: maybe apply non-home grip multiplier even if it's a different hand
        // for flipped positions?
        var shouldNonHomeGripApplyMultiplier = !motion.Type.IsWristMotion();
        if (shouldNonHomeGripApplyMultiplier && (currentHandOffset == HandOffset.ThumbOnD || currentHandOffset == HandOffset.ThumbOnU))
        {
            multipliers.Add((costConfig.OneFromHomeGripMultiplier, "one off from home grip"));
        }
        else if (shouldNonHomeGripApplyMultiplier && (currentHandOffset == HandOffset.FlippedTop || currentHandOffset == HandOffset.FlippedBottom))
        {
            multipliers.Add((costConfig.TwoFromHomeGripMultiplier, "flipped regrip"));
        }

        if (move.IsSliceMove)
        {
            multipliers.Add((costConfig.SliceMoveMultiplier, "slice move"));
        }

        if (lastNonWristMotion is not null && lastNonWristMotion.Hand == motion.Hand && lastNonWristMotion.Type.IsSameTypeAs(motion.Type))
        {
            penalties.Add((costConfig.OverworkingPenalty, "overworking"));
        }

        return new CostComponent(GetRawMotionCost(motion.Type, costConfig), multipliers, penalties);
    }

    private static double GetRawMotionCost(MotionType motionType, CostConfig costConfig)
    {
        return motionType switch
        {
            MotionType.WristUp or
            MotionType.WristDown or
            MotionType.DoubleWristUp or
            MotionType.DoubleWristDown => costConfig.WristTurnCost,
            MotionType.TripleWristUp or
            MotionType.TripleWristDown => costConfig.TripleWristTurnCost,
            MotionType.IndexPull => costConfig.IndexPullCost,
            MotionType.IndexPush => costConfig.IndexPushCost,
            MotionType.DoubleIndexPull => costConfig.DoubleIndexPullCost,
            MotionType.ThumbPull => costConfig.ThumbPullCost,
            MotionType.ThumbPush => costConfig.ThumbPushCost,
            MotionType.DoubleThumbPull => costConfig.DoubleThumbPullCost,
            MotionType.RingPull => costConfig.RingPullCost,
            MotionType.RingPush => costConfig.RingPushCost,
            MotionType.DoubleRingPull => costConfig.DoubleRingPullCost,
            MotionType.MiddlePull => costConfig.MiddlePullCost,
            MotionType.MiddlePush => costConfig.MiddlePushCost,
            MotionType.DoubleMiddlePull => costConfig.DoubleMiddlePullCost,
            _ => 0.0,
        };
    }
}
