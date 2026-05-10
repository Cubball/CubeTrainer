namespace CubeTrainer.Cube.Analyzer.State;

internal interface IStateTransition
{
    void Accept(IStateTransitionVisitor stateTransitionVisitor);
}
