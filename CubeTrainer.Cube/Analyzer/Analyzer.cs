using CubeTrainer.Cube.Analyzer.Models;
using CubeTrainer.Cube.Analyzer.State;

namespace CubeTrainer.Cube.Analyzer;

public static class Analyzer
{
    private static readonly Dictionary<AnalyzerState, double> States = [];
    private static readonly Dictionary<AnalyzerState, IStateTransition?> StateTransitions = [];
    private static readonly ApplyStateTransitionVisitor ApplyStateTransitionVisitor = new();
    private static readonly CostStateTransitionVisitor CostStateTransitionVisitor = new();
    private static readonly PrintStateTransitionVisitor PrintStateTransitionVisitor = new(CostStateTransitionVisitor);

    public static void Analzye(MoveSequence moveSequence)
    {
        var initState = new AnalyzerState(0, HandOffset.Home, HandOffset.Home, null, null);
        _ = GetCostFromState(initState, moveSequence); // pre-compute the cost and transitions

        while (StateTransitions.TryGetValue(initState, out var stateTransition) && stateTransition is not null)
        {
            stateTransition.Accept(PrintStateTransitionVisitor);
            Console.WriteLine(PrintStateTransitionVisitor.LastString);
            stateTransition.Accept(ApplyStateTransitionVisitor);
            var nextState = ApplyStateTransitionVisitor.LastState;
            if (nextState is null)
            {
                break;
            }

            initState = nextState;
        }
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
