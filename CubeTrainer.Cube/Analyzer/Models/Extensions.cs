namespace CubeTrainer.Cube.Analyzer.Models;

internal static class Extensions
{
    public static bool IsFingerMotion(this MotionType motionType)
    {
        return motionType
            is MotionType.IndexPull
            or MotionType.IndexPush
            or MotionType.DoubleIndexPull
            or MotionType.ThumbPush
            or MotionType.ThumbPull
            or MotionType.RingPull
            or MotionType.RingPush
            or MotionType.DoubleRingPull
            or MotionType.MiddlePull
            or MotionType.MiddlePush;
    }

    public static bool IsSameTypeAs(this MotionType motionType, MotionType otherMotionType)
    {
        if (motionType == otherMotionType)
        {
            return true;
        }

        return motionType switch
        {
            MotionType.IndexPull => otherMotionType is MotionType.DoubleIndexPull,
            MotionType.DoubleIndexPull => otherMotionType is MotionType.IndexPull,
            MotionType.RingPull => otherMotionType is MotionType.DoubleRingPull,
            MotionType.DoubleRingPull => otherMotionType is MotionType.RingPull,
            MotionType.MiddlePull => otherMotionType is MotionType.DoubleMiddlePull,
            MotionType.DoubleMiddlePull => otherMotionType is MotionType.MiddlePull,
            MotionType.ThumbPull => otherMotionType is MotionType.DoubleThumbPull,
            MotionType.DoubleThumbPull => otherMotionType is MotionType.ThumbPull,
            _ => false,
        };
    }

    public static bool IsWristMotion(this MotionType motionType)
    {
        return motionType
            is MotionType.WristUp
            or MotionType.WristDown
            or MotionType.DoubleWristUp
            or MotionType.DoubleWristDown
            or MotionType.TripleWristUp
            or MotionType.TripleWristDown;
    }
}
