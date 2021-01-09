
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for the system type <see cref="TimeSpan"/>
/// </summary>
public static class TimeSpanExtensions
{
    public static async Task SleepAsync(this TimeSpan duration) => await Task.Delay(duration);

    public static TimeSpan Divide(this TimeSpan timeSpan, double value)
    {
        return (timeSpan.TotalMilliseconds / value).Milliseconds();
    }

    /// <summary>
    /// Sleeps the amount of time.
    /// </summary>
    /// <param name="timeSpan">The time span value on which the extension is invoked.</param>
    public static void Sleep(this TimeSpan timeSpan)
    {
        Thread.Sleep(timeSpan);
    }
}