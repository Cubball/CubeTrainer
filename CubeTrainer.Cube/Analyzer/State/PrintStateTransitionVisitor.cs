using CubeTrainer.Cube.Analyzer.State;

internal class PrintStateTransitionVisitor : IStateTransitionVisitor
{
    public string LastString { get; private set; } = "";

    public void Visit(MotionStateTransition motionStateTransition)
    {
        LastString = $"{motionStateTransition.Move} - [{motionStateTransition.Motion.Hand} {motionStateTransition.Motion.Type}]";
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        LastString = $"[{regripStateTransition.Regrip.Hand} to {regripStateTransition.Regrip.NewHandOffset}]";
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        LastString = $"{rotationStateTransition.Move} - [rotate]";
    }
}
