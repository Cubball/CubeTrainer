internal interface IStateTransition
{
    void Accept(IStateTransitionVisitor stateTransitionVisitor);
}
