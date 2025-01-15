namespace CubeTrainer.API.Common.Helpers;

internal static class DateTimeHelpers
{
    public static DateTime UtcNow => new(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
}