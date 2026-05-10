using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal static class StateTransformer
{
    private static readonly Dictionary<(HandOffset, char), char> ToHomeGripMappings = new()
    {
        { (HandOffset.ThumbOnU, 'B'), 'U' },
        { (HandOffset.ThumbOnU, 'U'), 'F' },
        { (HandOffset.ThumbOnU, 'F'), 'D' },
        { (HandOffset.ThumbOnU, 'D'), 'B' },

        { (HandOffset.ThumbOnD, 'B'), 'D' },
        { (HandOffset.ThumbOnD, 'U'), 'B' },
        { (HandOffset.ThumbOnD, 'F'), 'U' },
        { (HandOffset.ThumbOnD, 'D'), 'F' },

        { (HandOffset.FlippedTop, 'B'), 'F' },
        { (HandOffset.FlippedTop, 'U'), 'D' },
        { (HandOffset.FlippedTop, 'F'), 'B' },
        { (HandOffset.FlippedTop, 'D'), 'U' },

        { (HandOffset.FlippedBottom, 'B'), 'F' },
        { (HandOffset.FlippedBottom, 'U'), 'D' },
        { (HandOffset.FlippedBottom, 'F'), 'B' },
        { (HandOffset.FlippedBottom, 'D'), 'U' },
    };

    public static List<IStateTransition> GetStateTransitions(AnalyzerState currentState, Move nextMove)
    {
        if (nextMove.IsRotation)
        {
            return [new RotationStateTransition(currentState, nextMove)];
        }


        List<IStateTransition> states = [
            .. GetPossibleRegrips(currentState.LeftHandOffset, Hand.Left).Select(r => new RegripStateTransition(currentState, r)),
            .. GetPossibleRegrips(currentState.RightHandOffset, Hand.Right).Select(r => new RegripStateTransition(currentState, r))
        ];
        if (nextMove.IsSliceMove)
        {
            // TODO:
            return states;
        }

        var face = char.ToUpperInvariant(nextMove.Face);
        if (nextMove.IsWristMove)
        {
            var motions = face == 'L' ? GetPossibleMotionsForLeftHand(nextMove) : GetPossibleMotionsForRightHand(nextMove);
            return [.. states, .. motions.Select(m => new MotionStateTransition(currentState, m))];
        }

        var success = ToHomeGripMappings.TryGetValue((currentState.LeftHandOffset, face), out var leftHandFace);
        if (success)
        {
            var motions = GetPossibleMotionsForLeftHand(new Move(leftHandFace, nextMove.Count));
            states.AddRange(motions.Select(m => new MotionStateTransition(currentState, m)));
        }

        success = ToHomeGripMappings.TryGetValue((currentState.RightHandOffset, face), out var rightHandFace);
        if (success)
        {
            var motions = GetPossibleMotionsForRightHand(new Move(rightHandFace, nextMove.Count));
            states.AddRange(motions.Select(m => new MotionStateTransition(currentState, m)));
        }

        return states;
    }

    private static List<Regrip> GetPossibleRegrips(HandOffset currentHandOffset, Hand hand)
    {
        var regrips = new List<Regrip>(4);
        for (var i = -2; i <= 2; i++)
        {
            if (i == (int)currentHandOffset)
            {
                continue;
            }

            regrips.Add(new Regrip(hand, (HandOffset)i));
        }

        return regrips;
    }

    private static List<Motion> GetPossibleMotionsForLeftHand(Move move)
    {
        return move switch
        {
            { Face: 'L', Count: 1 } => [new Motion(MotionType.WristDown, Hand.Left), new Motion(MotionType.TripleWristUp, Hand.Left)],
            { Face: 'L', Count: 2 } => [new Motion(MotionType.DoubleWristUp, Hand.Left), new Motion(MotionType.DoubleWristDown, Hand.Left)],
            { Face: 'L', Count: 3 } => [new Motion(MotionType.WristUp, Hand.Left), new Motion(MotionType.TripleWristDown, Hand.Left)],
            { Face: 'U', Count: 1 } => [new Motion(MotionType.IndexPush, Hand.Left)],
            { Face: 'U', Count: 2 } => [new Motion(MotionType.DoubleIndexPull, Hand.Left)],
            { Face: 'U', Count: 3 } => [new Motion(MotionType.IndexPull, Hand.Left)],
            { Face: 'F', Count: 1 } => [new Motion(MotionType.ThumbPush, Hand.Left)],
            { Face: 'F', Count: 2 } => [new Motion(MotionType.DoubleThumbPull, Hand.Left)],
            { Face: 'F', Count: 3 } => [new Motion(MotionType.ThumbPull, Hand.Left)],
            { Face: 'D', Count: 1 } => [new Motion(MotionType.RingPull, Hand.Left)],
            { Face: 'D', Count: 2 } => [new Motion(MotionType.DoubleRingPull, Hand.Left)],
            { Face: 'D', Count: 3 } => [new Motion(MotionType.RingPush, Hand.Left)],
            _ => [],
        };
    }

    private static List<Motion> GetPossibleMotionsForRightHand(Move move)
    {
        return move switch
        {
            { Face: 'R', Count: 1 } => [new Motion(MotionType.WristUp, Hand.Right), new Motion(MotionType.TripleWristDown, Hand.Right)],
            { Face: 'R', Count: 2 } => [new Motion(MotionType.DoubleWristUp, Hand.Right), new Motion(MotionType.DoubleWristDown, Hand.Right)],
            { Face: 'R', Count: 3 } => [new Motion(MotionType.WristDown, Hand.Right), new Motion(MotionType.TripleWristUp, Hand.Right)],
            { Face: 'U', Count: 1 } => [new Motion(MotionType.IndexPull, Hand.Right)],
            { Face: 'U', Count: 2 } => [new Motion(MotionType.DoubleIndexPull, Hand.Right)],
            { Face: 'U', Count: 3 } => [new Motion(MotionType.IndexPush, Hand.Right)],
            { Face: 'F', Count: 1 } => [new Motion(MotionType.ThumbPull, Hand.Right)],
            { Face: 'F', Count: 2 } => [new Motion(MotionType.DoubleThumbPull, Hand.Right)],
            { Face: 'F', Count: 3 } => [new Motion(MotionType.ThumbPush, Hand.Right)],
            { Face: 'D', Count: 1 } => [new Motion(MotionType.RingPush, Hand.Right)],
            { Face: 'D', Count: 2 } => [new Motion(MotionType.DoubleRingPull, Hand.Right)],
            { Face: 'D', Count: 3 } => [new Motion(MotionType.RingPull, Hand.Right)],
            _ => [],
        };
    }
}
