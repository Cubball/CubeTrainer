internal sealed record RotationStateTransition : IStateTransition
{
    public AnalyzerState? Apply(AnalyzerState state)
    {
        return state with
        {
            LeftHandOffset = HandOffset.Home,
            RightHandOffset = HandOffset.Home,
            LastNonWristMotion = null,
            MovesCompleted = state.MovesCompleted + 1,
        };
    }

    public double GetCost(AnalyzerState state)
    {
        return CostConfig.RotationCost;
    }
}
