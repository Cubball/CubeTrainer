using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal class ApplyStateTransitionVisitor : IStateTransitionVisitor
{
    public AnalyzerState? LastState { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        var state = motionStateTransition.CurrentState;
        var motion = motionStateTransition.Motion;
        var motionHandOffset = (int)state.RightHandOffset;
        if (state.LastHandRegrip is not null && motion.Hand != state.LastHandRegrip)
        {
            LastState = null;
            return;
        }

        if (motion.Hand == Hand.Left)
        {
            motionHandOffset = (int)state.LeftHandOffset;
        }

        var offsetDelta = GetHandOffsetDelta(motion.Type);
        var newOffset = motionHandOffset + offsetDelta;
        if (newOffset is > 2 or < -2)
        {
            LastState = null;
            return;
        }

        var lastNonWristMotion = motion.Type.IsWristMotion() ? state.LastNonWristMotion : motion;
        LastState = motion.Hand == Hand.Left
            ? (state with
            {
                LastNonWristMotion = lastNonWristMotion,
                MovesCompleted = state.MovesCompleted + 1,
                LeftHandOffset = (HandOffset)newOffset,
                LastHandRegrip = null,
            })
            : (state with
            {
                LastNonWristMotion = lastNonWristMotion,
                MovesCompleted = state.MovesCompleted + 1,
                RightHandOffset = (HandOffset)newOffset,
                LastHandRegrip = null,
            });
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        if (regripStateTransition.CurrentState.LastHandRegrip is not null)
        {
            LastState = null;
            return;
        }

        var regrip = regripStateTransition.Regrip;
        if (regrip.Hand == Hand.Left)
        {
            LastState = regripStateTransition.CurrentState with
            {
                LeftHandOffset = regrip.NewHandOffset,
                LastHandRegrip = regrip.Hand,
            };
        }
        else
        {
            LastState = regripStateTransition.CurrentState with
            {
                RightHandOffset = regrip.NewHandOffset,
                LastHandRegrip = regrip.Hand,
            };
        }
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        var state = rotationStateTransition.CurrentState;
        LastState = state with
        {
            LeftHandOffset = HandOffset.Home,
            RightHandOffset = HandOffset.Home,
            LastNonWristMotion = null,
            MovesCompleted = state.MovesCompleted + 1,
            LastHandRegrip = null,
        };
    }

    private static int GetHandOffsetDelta(MotionType motionType)
    {
        return motionType switch
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
