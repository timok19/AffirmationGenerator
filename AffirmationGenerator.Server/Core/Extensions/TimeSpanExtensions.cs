namespace AffirmationGenerator.Server.Core.Extensions;

public static class TimeSpanExtensions
{
    extension(TimeSpan)
    {
        public static TimeSpan OneDay => TimeSpan.FromDays(1);
    }
}
