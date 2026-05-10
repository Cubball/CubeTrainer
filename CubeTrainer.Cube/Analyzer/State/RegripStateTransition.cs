internal sealed record RegripStateTransition(
    AnalyzerState CurrentState,
    Hand Hand,
    HandOffset NewHandOffset) : IStateTransition
{
    public void Accept(IStateTransitionVisitor stateTransitionVisitor)
    {
        stateTransitionVisitor.Visit(this);
    }
}
