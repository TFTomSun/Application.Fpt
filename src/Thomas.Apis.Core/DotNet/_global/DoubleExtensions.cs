using System;


public static class DoubleExtensions
{

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of hours.
    /// </summary>
    /// <param name="value">The double value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of hours.</returns>
    public static TimeSpan Hours(this double value)
    {
        return TimeSpan.FromHours(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of minutes.
    /// </summary>
    /// <param name="value">The double value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of minutes.</returns>
    public static TimeSpan Minutes(this double value)
    {
        return TimeSpan.FromMinutes(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of seconds.
    /// </summary>
    /// <param name="value">The double value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of seconds.</returns>
    public static TimeSpan Seconds(this double value)
    {
        return TimeSpan.FromSeconds(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of milliseconds.
    /// </summary>
    /// <param name="value">The double value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of seconds.</returns>
    public static TimeSpan Milliseconds(this double value)
    {
        return TimeSpan.FromMilliseconds(value);
    }
}
