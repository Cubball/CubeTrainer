using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal sealed class ResultStateTransitionVisitor(CostStateTransitionVisitor costStateTransitionVisitor) : IStateTransitionVisitor
{
    private readonly CostStateTransitionVisitor _costStateTransitionVisitor = costStateTransitionVisitor;

    public AnalysisStep? LastStep { get; private set; }

    public void Visit(MotionStateTransition motionStateTransition)
    {
        _costStateTransitionVisitor.Visit(motionStateTransition);
        var cost = MapCost(_costStateTransitionVisitor.LastCost);
        LastStep = new MotionAnalysisStep(
            motionStateTransition.Move.ToString(),
            motionStateTransition.Motion.Hand.ToString(),
            motionStateTransition.Motion.Type.ToString(),
            cost);
    }

    public void Visit(RegripStateTransition regripStateTransition)
    {
        _costStateTransitionVisitor.Visit(regripStateTransition);
        var cost = MapCost(_costStateTransitionVisitor.LastCost);
        LastStep = new RegripAnalysisStep(
            regripStateTransition.Regrip.Hand.ToString(),
            regripStateTransition.Regrip.NewHandOffset.ToString(),
            cost);
    }

    public void Visit(RotationStateTransition rotationStateTransition)
    {
        _costStateTransitionVisitor.Visit(rotationStateTransition);
        var cost = MapCost(_costStateTransitionVisitor.LastCost);
        LastStep = new RotationAnalysisStep(
            rotationStateTransition.Move.ToString(),
            cost);
    }

    private static AnalysisStepCost MapCost(CostComponent? costComponent)
    {
        if (costComponent is null)
        {
            return new AnalysisStepCost(0, 0, [], []);
        }

        return new AnalysisStepCost(
            costComponent.TotalCost,
            costComponent.BaseCost,
            costComponent.Multipliers
                .Select(static m => new CostModifier(m.Value, m.Description))
                .ToList(),
            costComponent.Penalties
                .Select(static p => new CostModifier(p.Value, p.Description))
                .ToList());
    }
}
