using CubeTrainer.Cube.Analyzer.Models;
using CubeTrainer.Cube.Analyzer.State;

namespace CubeTrainer.Cube.Analyzer;

public static class Analyzer
{
    public static AnalysisResult Analyze(MoveSequence moveSequence, CostConfig? costConfig = null)
    {
        var resolvedCostConfig = costConfig ?? CostConfig.Default;
        var states = new Dictionary<AnalyzerState, double>();
        var stateTransitions = new Dictionary<AnalyzerState, IStateTransition?>();
        var applyStateTransitionVisitor = new ApplyStateTransitionVisitor();
        var costStateTransitionVisitor = new CostStateTransitionVisitor(resolvedCostConfig);
        var resultStateTransitionVisitor = new ResultStateTransitionVisitor(costStateTransitionVisitor);

        var initState = new AnalyzerState(0, HandOffset.Home, HandOffset.Home, null, null);
        var totalCost = GetCostFromState(
            initState,
            moveSequence,
            states,
            stateTransitions,
            applyStateTransitionVisitor,
            costStateTransitionVisitor);

        var steps = new List<AnalysisStep>();
        while (stateTransitions.TryGetValue(initState, out var stateTransition) && stateTransition is not null)
        {
            stateTransition.Accept(resultStateTransitionVisitor);
            if (resultStateTransitionVisitor.LastStep is not null)
            {
                steps.Add(resultStateTransitionVisitor.LastStep);
            }

            stateTransition.Accept(applyStateTransitionVisitor);
            var nextState = applyStateTransitionVisitor.LastState;
            if (nextState is null)
            {
                break;
            }

            initState = nextState;
        }

        return new AnalysisResult(totalCost, steps);
    }

    private static double GetCostFromState(
        AnalyzerState analyzerState,
        MoveSequence moveSequence,
        Dictionary<AnalyzerState, double> states,
        Dictionary<AnalyzerState, IStateTransition?> stateTransitions,
        ApplyStateTransitionVisitor applyStateTransitionVisitor,
        CostStateTransitionVisitor costStateTransitionVisitor)
    {
        var movesCompleted = analyzerState.MovesCompleted;
        if (movesCompleted == moveSequence.Moves.Count)
        {
            return 0;
        }

        if (states.TryGetValue(analyzerState, out var cost))
        {
            return cost;
        }

        var nextStateTransitions = StateTransformer.GetStateTransitions(analyzerState, moveSequence.Moves[movesCompleted]);
        // HACK: better use nullable double
        var minCost = double.MaxValue;
        foreach (var stateTransition in nextStateTransitions)
        {
            stateTransition.Accept(costStateTransitionVisitor);
            var transitionCost = costStateTransitionVisitor.LastCost;
            stateTransition.Accept(applyStateTransitionVisitor);
            var nextState = applyStateTransitionVisitor.LastState;
            if (nextState is null)
            {
                continue;
            }

            var nextStateCost = GetCostFromState(
                nextState,
                moveSequence,
                states,
                stateTransitions,
                applyStateTransitionVisitor,
                costStateTransitionVisitor);
            var currentCost = nextStateCost + (transitionCost?.TotalCost ?? double.MaxValue);
            if (currentCost < minCost)
            {
                minCost = currentCost;
                stateTransitions[analyzerState] = stateTransition;
            }
        }

        return states[analyzerState] = minCost;
    }
}
