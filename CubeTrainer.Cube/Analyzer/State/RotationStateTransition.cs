namespace CubeTrainer.Cube.Analyzer.State;

internal sealed record RotationStateTransition(
    AnalyzerState CurrentState,
    Move Move) : IStateTransition
{
    public void Accept(IStateTransitionVisitor stateTransitionVisitor)
    {
        stateTransitionVisitor.Visit(this);
    }
}
