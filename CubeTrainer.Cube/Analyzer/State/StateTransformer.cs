using CubeTrainer.Cube.Analyzer.Models;

namespace CubeTrainer.Cube.Analyzer.State;

internal static class StateTransformer
{
    private static readonly Dictionary<(HandOffset, char), char> ToHomeGripMappings = new()
    {
        { (HandOffset.Home, 'B'), 'B' },
        { (HandOffset.Home, 'U'), 'U' },
        { (HandOffset.Home, 'F'), 'F' },
        { (HandOffset.Home, 'D'), 'D' },
        { (HandOffset.Home, 'R'), 'R' },
        { (HandOffset.Home, 'L'), 'L' },
        { (HandOffset.Home, 'M'), 'M' },
        { (HandOffset.Home, 'E'), 'E' },
        { (HandOffset.Home, 'S'), 'S' },

        { (HandOffset.ThumbOnU, 'B'), 'U' },
        { (HandOffset.ThumbOnU, 'U'), 'F' },
        { (HandOffset.ThumbOnU, 'F'), 'D' },
        { (HandOffset.ThumbOnU, 'D'), 'B' },
        { (HandOffset.ThumbOnU, 'R'), 'R' },
        { (HandOffset.ThumbOnU, 'L'), 'L' },
        { (HandOffset.ThumbOnU, 'M'), 'M' },
        { (HandOffset.ThumbOnU, 'E'), 'S' },
        { (HandOffset.ThumbOnU, 'S'), 'E' },

        { (HandOffset.ThumbOnD, 'B'), 'D' },
        { (HandOffset.ThumbOnD, 'U'), 'B' },
        { (HandOffset.ThumbOnD, 'F'), 'U' },
        { (HandOffset.ThumbOnD, 'D'), 'F' },
        { (HandOffset.ThumbOnD, 'R'), 'R' },
        { (HandOffset.ThumbOnD, 'L'), 'L' },
        { (HandOffset.ThumbOnD, 'M'), 'M' },
        { (HandOffset.ThumbOnD, 'E'), 'S' },
        { (HandOffset.ThumbOnD, 'S'), 'E' },

        { (HandOffset.FlippedTop, 'B'), 'F' },
        { (HandOffset.FlippedTop, 'U'), 'D' },
        { (HandOffset.FlippedTop, 'F'), 'B' },
        { (HandOffset.FlippedTop, 'D'), 'U' },
        { (HandOffset.FlippedTop, 'R'), 'R' },
        { (HandOffset.FlippedTop, 'L'), 'L' },
        { (HandOffset.FlippedTop, 'M'), 'M' },
        { (HandOffset.FlippedTop, 'E'), 'E' },
        { (HandOffset.FlippedTop, 'S'), 'S' },

        { (HandOffset.FlippedBottom, 'B'), 'F' },
        { (HandOffset.FlippedBottom, 'U'), 'D' },
        { (HandOffset.FlippedBottom, 'F'), 'B' },
        { (HandOffset.FlippedBottom, 'D'), 'U' },
        { (HandOffset.FlippedBottom, 'R'), 'R' },
        { (HandOffset.FlippedBottom, 'L'), 'L' },
        { (HandOffset.FlippedBottom, 'M'), 'M' },
        { (HandOffset.FlippedBottom, 'E'), 'E' },
        { (HandOffset.FlippedBottom, 'S'), 'S' },
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
        var face = char.ToUpperInvariant(nextMove.Face);
        var success = ToHomeGripMappings.TryGetValue((currentState.LeftHandOffset, face), out var leftHandFace);
        if (success)
        {
            var move = GetModifiedMove(new(leftHandFace, nextMove.Count), currentState.LeftHandOffset);
            var motions = GetPossibleMotionsForLeftHand(move);
            states.AddRange(motions.Select(m => new MotionStateTransition(currentState, m, nextMove)));
        }

        success = ToHomeGripMappings.TryGetValue((currentState.RightHandOffset, face), out var rightHandFace);
        if (success)
        {
            var move = GetModifiedMove(new(rightHandFace, nextMove.Count), currentState.RightHandOffset);
            var motions = GetPossibleMotionsForRightHand(move);
            states.AddRange(motions.Select(m => new MotionStateTransition(currentState, m, nextMove)));
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
            { Face: 'S', Count: 1 } => [new Motion(MotionType.IndexPush, Hand.Left)],
            { Face: 'S', Count: 2 } => [new Motion(MotionType.DoubleIndexPull, Hand.Left)],
            { Face: 'S', Count: 3 } => [new Motion(MotionType.IndexPull, Hand.Left)],
            { Face: 'E', Count: 1 } => [new Motion(MotionType.MiddlePull, Hand.Left)],
            { Face: 'E', Count: 2 } => [new Motion(MotionType.DoubleMiddlePull, Hand.Left)],
            { Face: 'E', Count: 3 } => [new Motion(MotionType.MiddlePush, Hand.Left)],
            { Face: 'M', Count: 1 } => [new Motion(MotionType.RingPush, Hand.Left)],
            { Face: 'M', Count: 2 } => [new Motion(MotionType.DoubleRingPull, Hand.Left)],
            { Face: 'M', Count: 3 } => [new Motion(MotionType.RingPull, Hand.Left)],
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
            { Face: 'S', Count: 1 } => [new Motion(MotionType.IndexPull, Hand.Right)],
            { Face: 'S', Count: 2 } => [new Motion(MotionType.DoubleIndexPull, Hand.Right)],
            { Face: 'S', Count: 3 } => [new Motion(MotionType.IndexPush, Hand.Right)],
            { Face: 'E', Count: 1 } => [new Motion(MotionType.MiddlePush, Hand.Right)],
            { Face: 'E', Count: 2 } => [new Motion(MotionType.DoubleMiddlePull, Hand.Right)],
            { Face: 'E', Count: 3 } => [new Motion(MotionType.MiddlePull, Hand.Right)],
            { Face: 'M', Count: 1 } => [new Motion(MotionType.RingPush, Hand.Right)],
            { Face: 'M', Count: 2 } => [new Motion(MotionType.DoubleRingPull, Hand.Right)],
            { Face: 'M', Count: 3 } => [new Motion(MotionType.RingPull, Hand.Right)],
            _ => [],
        };
    }

    private static Move GetModifiedMove(Move move, HandOffset currentHandOffset)
    {
        return ShouldFlipDirection(move, currentHandOffset) ? move.Inverse() : move;
    }

    private static bool ShouldFlipDirection(Move move, HandOffset currentHandOffset)
    {
        if (!move.IsSliceMove)
        {
            return false;
        }

        if (move.Face == 'M' || currentHandOffset is HandOffset.Home)
        {
            return false;
        }

        if (currentHandOffset is HandOffset.FlippedTop or HandOffset.FlippedBottom)
        {
            return true;
        }

        if (move.Face == 'S')
        {
            return currentHandOffset == HandOffset.ThumbOnD;
        }

        if (move.Face == 'E')
        {
            return currentHandOffset == HandOffset.ThumbOnU;
        }

        return false;
    }
}
