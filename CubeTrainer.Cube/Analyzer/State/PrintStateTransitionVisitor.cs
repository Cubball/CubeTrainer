using CubeTrainer.Cube.Analyzer.State;

internal class PrintStateTransitionVisitor(CostStateTransitionVisitor costStateTransitionVisitor) : IStateTransitionVisitor
{
    private readonly CostStateTransitionVisitor _costStateTransitionVisitor = costStateTransitionVisitor;

    public string LastString { get; private set; } = "";

    public void Visit(MotionStateTransition motionStateTransition)
    {
        _costStateTransitionVisitor.Visit(motionStateTransition);
        var cost = _costStateTransitionVisitor.LastCost;
        LastString = $"{motionStateTransition.Move} - {motionStateTransition.Motion.Hand} {motionStateTransition.Motion.Type} ({cost})";
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        _costStateTransitionVisitor.Visit(regripStateTransition);
        var cost = _costStateTransitionVisitor.LastCost;
        LastString = $"{regripStateTransition.Regrip.Hand} to {regripStateTransition.Regrip.NewHandOffset} ({cost})";
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        _costStateTransitionVisitor.Visit(rotationStateTransition);
        var cost = _costStateTransitionVisitor.LastCost;
        LastString = $"{rotationStateTransition.Move} - ({cost})";
    }
}
