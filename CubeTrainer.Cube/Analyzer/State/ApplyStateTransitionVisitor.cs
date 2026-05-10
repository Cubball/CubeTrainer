internal class ApplyStateTransitionVisitor : IStateTransitionVisitor
{
    public AnalyzerState? LastState { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        var state = motionStateTransition.CurrentState;
        var motion = motionStateTransition.Motion;
        var motionHandOffset = (int)state.RightHandOffset;
        if (motion.Hand == Hand.Left)
        {
            motionHandOffset = (int)state.LeftHandOffset;
        }

        var offsetDelta = GetHandOffsetDelta(motion.Type);
        var newOffset = motionHandOffset + offsetDelta;
        if (newOffset is > 2 or < -2)
        {
            LastState = null;
        }

        LastState = motion.Hand == Hand.Left
            ? (state with
            {
                LastNonWristMotion = motion,
                MovesCompleted = state.MovesCompleted + 1,
                LeftHandOffset = (HandOffset)newOffset,
            })
            : (state with
            {
                LastNonWristMotion = motion,
                MovesCompleted = state.MovesCompleted + 1,
                RightHandOffset = (HandOffset)newOffset,
            });
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        if (regripStateTransition.Hand == Hand.Left)
        {
            LastState = regripStateTransition.CurrentState with
            {
                LeftHandOffset = regripStateTransition.NewHandOffset,
            };
        }
        else
        {
            LastState = regripStateTransition.CurrentState with
            {
                RightHandOffset = regripStateTransition.NewHandOffset,
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
