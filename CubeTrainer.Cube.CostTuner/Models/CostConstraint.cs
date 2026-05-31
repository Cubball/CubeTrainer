namespace CubeTrainer.Cube.CostTuner.Models;

internal sealed record CostConstraint(
    string LeftProperty,
    ConstraintRelation Relation,
    string RightProperty);
