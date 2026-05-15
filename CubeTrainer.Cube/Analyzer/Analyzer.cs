using CubeTrainer.Cube.Analyzer.Models;
using CubeTrainer.Cube.Analyzer.State;

namespace CubeTrainer.Cube.Analyzer;

public static class Analyzer
{
    private static readonly Dictionary<AnalyzerState, double> States = [];
    private static readonly Dictionary<AnalyzerState, IStateTransition?> StateTransitions = [];
    private static readonly ApplyStateTransitionVisitor ApplyStateTransitionVisitor = new();
    private static readonly CostStateTransitionVisitor CostStateTransitionVisitor = new();
    private static readonly ResultStateTransitionVisitor ResultStateTransitionVisitor = new(CostStateTransitionVisitor);

    public static AnalysisResult Analyze(MoveSequence moveSequence)
    {
        States.Clear();
        StateTransitions.Clear();

        var initState = new AnalyzerState(0, HandOffset.Home, HandOffset.Home, null, null);
        var totalCost = GetCostFromState(initState, moveSequence);

        var steps = new List<AnalysisStep>();
        while (StateTransitions.TryGetValue(initState, out var stateTransition) && stateTransition is not null)
        {
            stateTransition.Accept(ResultStateTransitionVisitor);
            if (ResultStateTransitionVisitor.LastStep is not null)
            {
                steps.Add(ResultStateTransitionVisitor.LastStep);
            }

            stateTransition.Accept(ApplyStateTransitionVisitor);
            var nextState = ApplyStateTransitionVisitor.LastState;
            if (nextState is null)
            {
                break;
            }

            initState = nextState;
        }

        return new AnalysisResult(totalCost, steps);
    }

    private static double GetCostFromState(AnalyzerState analyzerState, MoveSequence moveSequence)
    {
        var movesCompleted = analyzerState.MovesCompleted;
        if (movesCompleted == moveSequence.Moves.Count)
        {
            return 0;
        }

        if (States.TryGetValue(analyzerState, out var cost))
        {
            return cost;
        }

        var nextStateTransitions = StateTransformer.GetStateTransitions(analyzerState, moveSequence.Moves[movesCompleted]);
        // HACK: better use nullable double
        var minCost = double.MaxValue;
        foreach (var stateTransition in nextStateTransitions)
        {
            stateTransition.Accept(CostStateTransitionVisitor);
            var transitionCost = CostStateTransitionVisitor.LastCost;
            stateTransition.Accept(ApplyStateTransitionVisitor);
            var nextState = ApplyStateTransitionVisitor.LastState;
            if (nextState is null)
            {
                continue;
            }

            var nextStateCost = GetCostFromState(nextState, moveSequence);
            var currentCost = nextStateCost + (transitionCost?.TotalCost ?? double.MaxValue);
            if (currentCost < minCost)
            {
                minCost = currentCost;
                StateTransitions[analyzerState] = stateTransition;
            }
        }

        return States[analyzerState] = minCost;
    }
}
