internal static class Extensions
{
    public static bool IsFingerMotion(this MotionType motionType)
    {
        return motionType
            is MotionType.IndexPull
            or MotionType.IndexPush
            or MotionType.IndexDoublePull
            or MotionType.ThumbPush
            or MotionType.ThumbPull
            or MotionType.RingPull
            or MotionType.RingPush
            or MotionType.DoubleRingPull
            or MotionType.MiddlePull
            or MotionType.MiddlePush;
    }

    public static bool IsInverseOf(this MotionType motionType, MotionType otherMotionType)
    {
        // We don't care about wrist motions
        // since this would be used to track
        // last non-wrist motion
        if (!motionType.IsFingerMotion())
        {
            return false;
        }

        return motionType switch
        {
            MotionType.IndexPull => otherMotionType is MotionType.IndexPush,
            MotionType.IndexPush => otherMotionType is MotionType.IndexPull,
            MotionType.ThumbPush => otherMotionType is MotionType.ThumbPull,
            MotionType.ThumbPull => otherMotionType is MotionType.ThumbPush,
            MotionType.RingPull => otherMotionType is MotionType.RingPush,
            MotionType.RingPush => otherMotionType is MotionType.RingPull,
            MotionType.MiddlePull => otherMotionType is MotionType.MiddlePush,
            MotionType.MiddlePush => otherMotionType is MotionType.MiddlePull,
            _ => false,
        };
    }
}
