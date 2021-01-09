
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

/// <summary>
/// Provides extension methods for integers.
/// </summary>
public static class IntExtensions
{
    public static double Mega(this long value)
    {
        return value / 1000000.0;
    }
    public static double Giga(this long value)
    {
        return value / 1000000000.0;
    }

    /// <summary>
    /// Determines whether the value is between the two boundaries.
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="lowerBound">The lower boundary.</param>
    /// <param name="higherBound">The upper boundary.</param>
    /// <param name="includeBoundaries">Determines whether to include the boundaries into the check.</param>
    /// <returns>true if the value is within (or on the boundaries, dependending on the given parameters).</returns>
    public static bool IsBetween(this int value, int lowerBound, int higherBound, bool includeBoundaries = false)
    {
        if (includeBoundaries)
        {
            var result = lowerBound <= value && value <= higherBound;
            return result;

        }
        else
        {
            var result = lowerBound < value && value < higherBound;
            return result;
        }
    }
    /// <summary>
    /// Converts an int value to a color value. If the value is 0, Color.Empty will be returned
    /// </summary>
    /// <param name="argbValue">The ARGB value of the color.</param>
    /// <param name="isRgbValue">Is in an rgb value. Default: true.</param>
    /// <returns>The color wit the received ARGB value, or Color.Empty for 0.</returns>
    public static Color ToColor(this int argbValue, bool isRgbValue = false)
    {
        var baseColor = Color.FromArgb(argbValue);
        var color = argbValue == 0 ? Color.Empty : isRgbValue ? Color.FromArgb(255, baseColor) : baseColor;
        return color;
    }

    /// <summary>
    /// Creates a sequence with the specified count.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <returns>The sequence from 0 to count.</returns>
    public static IEnumerable<int> ToRange(this int count)
    {
        return Enumerable.Range(0, count);
    }

    /// <summary>
    /// Determines whether the number is even.
    /// </summary>
    /// <param name="value">The number.</param>
    /// <returns>true, if the number is even, otherwise false.</returns>
    public static bool IsEven(this int value)
    {
        return value%2 == 0;
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of hours.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of hours.</returns>
    public static TimeSpan Hours(this int value)
    {
        return TimeSpan.FromHours(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of minutes.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of minutes.</returns>
    public static TimeSpan Minutes(this int value)
    {
        return TimeSpan.FromMinutes(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of seconds.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of seconds.</returns>
    public static TimeSpan Seconds(this int value)
    {
        return TimeSpan.FromSeconds(value);
    }

    /// <summary>
    /// Get the date time since defined time span.
    /// </summary>
    /// <param name="timeSpan">Time span since.</param>
    /// <returns></returns>
    public static DateTime Ago(this TimeSpan timeSpan)
    {
        return DateTime.Now - timeSpan;
    }

    /// <summary>
    /// Get the date time since defined time span.
    /// </summary>
    /// <param name="timeSpan">Time span since.</param>
    /// <returns></returns>
    public static DateTime InFuture(this TimeSpan timeSpan)
    {
        return DateTime.Now + timeSpan;
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of days.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of days.</returns>
    public static TimeSpan Years(this int value)
    {
        var maxDateTime = DateTime.MaxValue;
    
        return maxDateTime - new DateTime(maxDateTime.Year - value, maxDateTime.Month, maxDateTime.Day,maxDateTime.Hour,maxDateTime.Minute,maxDateTime.Second,maxDateTime.Millisecond);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of weeks.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of weeks.</returns>
    public static TimeSpan Weeks(this int value)
    {
        return TimeSpan.FromDays(value*7);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of days.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of days.</returns>
    public static TimeSpan Days(this int value)
    {
        return TimeSpan.FromDays(value);
    }

    /// <summary>
    /// Creates a TimeSpan value where the input value represents the number of milliseconds.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>TimeSpan value where the input value represents the number of milliseconds.</returns>
    public static TimeSpan Milliseconds(this int value)
    {
        return TimeSpan.FromMilliseconds(value);
    }

    /// <summary>
    /// Transforms the amount of megabytes received as parameter in the corresponding number of bytes.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>The number of bytes corresponding to the megabytes received.</returns>
    public static long MegaByte(this int value)
    {
        return (long) Math.Pow(2, 20)*value;
    }

    /// <summary>
    /// Transforms the amount of kilobytes received as parameter in the corresponding number of bytes.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>The number of bytes corresponding to the kilobytes received.</returns>
    public static long KiloByte(this int value)
    {
        return (long) Math.Pow(2, 10)*value;
    }

    /// <summary>
    /// Transforms the amount of gigabytes received as parameter in the corresponding number of bytes.
    /// </summary>
    /// <param name="value">The int32 value on which the extension is invoked.</param>
    /// <returns>The number of bytes corresponding to the gigabytes received.</returns>
    public static long GigaByte(this int value)
    {
        return (long) Math.Pow(2, 30)*value;
    }

    /// <summary>
    /// Converts an integer number into its binary representation on a specific format. 
    /// If the specified length is not reached, it will be padded with "0"s.
    /// </summary>
    /// <param name="value">The numeric value to be converted</param>
    /// <param name="length">The desired length of the string.</param>
    /// <returns>Binary representation of the parameter </returns>
    public static string ToBinaryString(this Int32 value, int length = -1)
    {
        var binValueString = Convert.ToString(value, 2);
        if (length != -1)
        {
            var initialLength = binValueString.Length;
            for (var i = initialLength; i < length; i++)
            {
                binValueString = "0" + binValueString;
            }
        }
        return binValueString;
    }

    ///// <summary>
    ///// Converts an integer value representing miliseconds into a long value representing ticks.
    ///// </summary>
    ///// <param name="milisecondsValue">The numeric value to be converted</param>
    ///// <returns>The converted value</returns>
    //public static long ToTicks(this int milisecondsValue)
    //{
    //    return milisecondsValue*10000;
    //}

    /// <summary>
    /// Get the next decimal number
    /// </summary>
    /// <param name="value">Current decimal number</param>
    /// <returns>Next decimal number</returns>
    public static int GetNextDecimalNumber(this int value)
    {
        return (int) Math.Pow(10, Math.Floor(Math.Log10(value) + 1));
    }
}