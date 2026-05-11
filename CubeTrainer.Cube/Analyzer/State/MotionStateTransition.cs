using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal sealed record MotionStateTransition(
    AnalyzerState CurrentState,
    Motion Motion,
    Move Move) : IStateTransition
{
    public void Accept(IStateTransitionVisitor stateTransitionVisitor)
    {
        stateTransitionVisitor.Visit(this);
    }
}
