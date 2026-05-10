internal sealed record RegripStateTransition(
    AnalyzerState CurrentState,
    Regrip Regrip) : IStateTransition
{
    public void Accept(IStateTransitionVisitor stateTransitionVisitor)
    {
        stateTransitionVisitor.Visit(this);
    }
}
