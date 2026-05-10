namespace CubeTrainer.Cube.Analyzer.State;

internal interface IStateTransitionVisitor
{
    void Visit(MotionStateTransition motionStateTransition);

    void Visit(RegripStateTransition regripStateTransition);

    void Visit(RotationStateTransition rotationStateTransition);
}
