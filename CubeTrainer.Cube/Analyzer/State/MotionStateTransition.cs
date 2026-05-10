internal sealed record MotionStateTransition(
    AnalyzerState CurrentState,
    Motion Motion) : IStateTransition
{
    public void Accept(IStateTransitionVisitor stateTransitionVisitor)
    {
        stateTransitionVisitor.Visit(this);
    }
}
