internal interface IStateTransition
{
    AnalyzerState? Apply(AnalyzerState state);

    double GetCost(AnalyzerState state);
}
