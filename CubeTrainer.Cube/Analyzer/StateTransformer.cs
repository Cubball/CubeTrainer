using CubeTrainer.Cube;

internal static class StateTransformer
{
    private static readonly Dictionary<(HandOffset, char), char> ToHomeGripMappings = new()
    {
        { (HandOffset.Home, 'R'), 'R' },
        { (HandOffset.Home, 'U'), 'U' },
        { (HandOffset.Home, 'F'), 'F' },
        { (HandOffset.Home, 'D'), 'D' },
        { (HandOffset.Home, 'L'), 'L' },
        { (HandOffset.Home, 'B'), 'B' },

        { (HandOffset.ThumbOnU, 'R'), 'R' },
        { (HandOffset.ThumbOnU, 'B'), 'U' },
        { (HandOffset.ThumbOnU, 'U'), 'F' },
        { (HandOffset.ThumbOnU, 'F'), 'D' },
        { (HandOffset.ThumbOnU, 'L'), 'L' },
        { (HandOffset.ThumbOnU, 'D'), 'B' },

        { (HandOffset.ThumbOnD, 'R'), 'R' },
        { (HandOffset.ThumbOnD, 'B'), 'D' },
        { (HandOffset.ThumbOnD, 'U'), 'B' },
        { (HandOffset.ThumbOnD, 'F'), 'U' },
        { (HandOffset.ThumbOnD, 'L'), 'L' },
        { (HandOffset.ThumbOnD, 'D'), 'F' },

        { (HandOffset.FlippedTop, 'R'), 'R' },
        { (HandOffset.FlippedTop, 'B'), 'F' },
        { (HandOffset.FlippedTop, 'U'), 'D' },
        { (HandOffset.FlippedTop, 'F'), 'B' },
        { (HandOffset.FlippedTop, 'L'), 'L' },
        { (HandOffset.FlippedTop, 'D'), 'U' },

        { (HandOffset.FlippedBottom, 'R'), 'R' },
        { (HandOffset.FlippedBottom, 'B'), 'F' },
        { (HandOffset.FlippedBottom, 'U'), 'D' },
        { (HandOffset.FlippedBottom, 'F'), 'B' },
        { (HandOffset.FlippedBottom, 'L'), 'L' },
        { (HandOffset.FlippedBottom, 'D'), 'U' },

    };

    public static List<Motion> GetAvailableMotions(AnalyzerState state, Move move)
    {
        // handle rotations
        // handle wrist motions
        // handle finger motins
        throw new NotImplementedException();
    }

    public static List<Regrip> GetAvailableRegrips(AnalyzerState state)
    {
        // handle regrips
        throw new NotImplementedException();
    }

    public static AnalyzerState ApplyMotion(AnalyzerState state, Motion motion)
    {

        throw new NotImplementedException();
    }

    public static AnalyzerState ApplyRegrip(AnalyzerState state, Regrip regrip)
    {
        throw new NotImplementedException();
    }

    private static Move GetToHomeGripMove(Hand hand, HandOffset handOffset, Move move)
    {
        throw new NotImplementedException();
    }
}
