internal sealed record RegripStateTransition(Hand Hand, HandOffset NewHandOffset) : IStateTransition
{
    public AnalyzerState? Apply(AnalyzerState state)
    {
        if (Hand == Hand.Left)
        {
            return state with
            {
                LeftHandOffset = NewHandOffset,
            };
        }
        else if (Hand == Hand.Right)
        {
            return state with
            {
                RightHandOffset = NewHandOffset,
            };
        }

        throw new InvalidOperationException($"Invalid hand: {Hand}");
    }

    public double GetCost(AnalyzerState state)
    {
        return NewHandOffset == HandOffset.FlippedBottom || NewHandOffset == HandOffset.FlippedTop
            ? CostConfig.FlippedRegripCost
            : CostConfig.RegripCost;
    }
}
