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
            _ => false,
        };
    }
}
