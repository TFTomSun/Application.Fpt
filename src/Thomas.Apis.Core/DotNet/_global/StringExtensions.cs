using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Thomas.Apis.Core;
using Thomas.Apis.Core.DotNet;

/// <summary>
/// Provides extension methods for the <see cref="String"/> class
/// </summary>
public static class StringExtensions
{
    public static void OpenInBrowser(this Uri  url)
    {
        var urlString = url.ToString();

        if (Environment.Version.Major == 4)
        {
            Process.Start(urlString);

        }
        else
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                urlString = urlString.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", urlString);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", urlString);
            }
            else
            {
                throw Api.Create.Exception("Couldn't open the url in the browser");
            }
        }
    }


    /// <summary>
    /// Converts the specified string to the specified enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="instance">The string on which the extension is invoked.</param>
    /// <param name="includeDescriptionParsing">Determines whether to include System.ComponentModel.Description attributes for parsing.</param>
    /// <param name="equalEvaluator">A custom equal evaluator to provide generic string to enum mapping.</param>
    /// <returns>The enum value.</returns>
    public static TEnum ToEnum<TEnum>(this String instance, bool includeDescriptionParsing = false,
        Func<String, TEnum, bool> equalEvaluator = null)
        where TEnum : struct, IComparable, IConvertible, IFormattable
    {
        if (instance == null) throw new ArgumentNullException("instance");

        var nullableEnum = instance.TryToEnum<TEnum>(includeDescriptionParsing, equalEvaluator);
        if (nullableEnum == null)
        {
            throw Api.Create.Exception(
                "The string value '{0}' could not be converted into one value of the enum type '{1}'.",
                instance, typeof(TEnum).Name);
        }

        return nullableEnum.Value;
    }
    /// <summary>
    /// Converts the specified string to the specified enum.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="instance">The string on which the extension is invoked.</param>
    /// <param name="includeDescriptionParsing">Determines whether to include System.ComponentModel.Description attributes for parsing.</param>
    /// <param name="equalEvaluator">A custom equal evaluator to provide generic string to enum mapping.</param>
    /// <returns>The enum value.</returns>
    public static TEnum? TryToEnum<TEnum>(this String instance, bool includeDescriptionParsing = false,
        Func<String, TEnum, bool> equalEvaluator = null)
        where TEnum : struct, IComparable, IConvertible, IFormattable
    {
        if (instance == null) return null;

        var nullableEnum = default(TEnum).TryParse(instance, includeDescriptionParsing, false, equalEvaluator);
        return nullableEnum;
    }
    /// <summary>
    /// Converts the specified value to a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of the value type.</typeparam>
    /// <param name="value">The instance of the value type.</param>
    /// <returns>The nullable value.</returns>
    public static T? ToNullable<T>(this T value)
        where T : struct
    {
        return value;
    }
    /// <summary>
    /// Tries to parse the specified enum string. Returns null if the string could not be converted into a valid enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="defaultValue">The default value, on which the extension is invoked.</param>
    /// <param name="enumString">The string that should be parsed into a enum value.</param>
    /// <param name="includeDescriptionParsing">Should description parse also</param>
    /// <param name="ignoreCase">Should case ignore</param>
    /// <param name="equalEvaluator">A custom equal evaluator to provide generic string to enum mapping.</param>
    /// <returns>The enum value or null, if the string could not be parsed.</returns>
    public static TEnum? TryParse<TEnum>(this TEnum defaultValue, string enumString,
        bool includeDescriptionParsing = false, bool ignoreCase = false, Func<String, TEnum, bool> equalEvaluator = null)
        where TEnum : struct, IConvertible, IFormattable, IComparable
    {
        TEnum result;
        if (Enum.TryParse(enumString, ignoreCase, out result))
        {
            return result;
        }

        if (includeDescriptionParsing)
        {
            var field = typeof(TEnum).GetFields().SingleOrDefault(
                f => f.GetCustomAttributes<DescriptionAttribute>().Select(
                    da => da.Description).SingleOrDefault() == enumString);

            if (field != null)
            {
                return defaultValue.TryParse(field.Name);
            }
        }

        if (equalEvaluator != null)
        {
            var value = defaultValue.GetValues().Select(v => v.ToNullable()).SingleOrDefault(
                v => equalEvaluator(enumString, v.GetValueOrDefault()));

            if (value != null)
            {
                return value.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Tries to convert the given string into a version instance. If failed, returns null.
    /// </summary>
    /// <param name="value">The string on which the extension is invoked.</param>
    /// <returns>The Version instance, or null if the conversion failed.</returns>
    public static Version TryToVersion(this string value)
    {
        if (Version.TryParse(value, out var version))
        {
            return version;
        }
        return null;
    }

    /// <summary>
    /// Extracts the values that are represented by the wildcards in a matching string.
    /// </summary>
    /// <param name="value">The string value which is expected to match with the wildcard pattern</param>
    /// <param name="wildcardPattern">The wildcard pattern</param>
    /// <returns>The parts of the string value which are represented as wildcard placeholders  in the wildcard pattern.</returns>
    public static string[] ExtractWildcardPlaceHolder(this string value, string wildcardPattern)
    {
        var patterns = wildcardPattern.Split('*');
        if (patterns.Last().Equals(""))
            patterns = patterns.Take(patterns.Length - 1).ToArray();
        string expression = string.Join("|", patterns);
        var wildCards = Regex.Replace(value, expression, " ").Trim().Split(' ');
        return wildCards;
    }


    /// <summary>
    /// Converts a numeric string into its corresponding Int32 value.
    /// </summary>
    /// <param name="value">String containing the numeric value.</param>
    /// <param name="fromBase">The base in which the number is represented. Ex (100, NumericFormat.Binary)=>4.</param>
    /// <returns>The numeric value representation of the string</returns>
    public static Int32 ToInt32(this String value, NumericFormat? fromBase = null)
    {
        UpdateFormat(value, ref fromBase);

        switch (fromBase)
        {
            case NumericFormat.Decimal:
                return Convert.ToInt32(value);
            case NumericFormat.Binary:
                return Convert.ToInt32(value, 2);
            case NumericFormat.Octal:
                return Convert.ToInt32(value, 8);
            case NumericFormat.Hexadecimal:
                return Convert.ToInt32(value, 16);
            default:
                throw Api.Create.Exception("Unexpected Value for base: '{0}'", fromBase);
        }
    }
    /// <summary>
    /// Converts a numeric string into its corresponding Int65 value.
    /// </summary>
    /// <param name="value">String containing the numeric value.</param>
    /// <param name="fromBase">The base in which the number is represented. Ex (100, NumericFormat.Binary)=>4.</param>
    /// <returns>The numeric value representation of the string</returns>
    public static Int64 ToInt64(this String value, NumericFormat? fromBase = null)
    {
        UpdateFormat(value, ref fromBase);

        switch (fromBase)
        {
            case NumericFormat.Decimal:
                return Convert.ToInt64(value);
            case NumericFormat.Binary:
                return Convert.ToInt64(value, 2);
            case NumericFormat.Octal:
                return Convert.ToInt64(value, 8);
            case NumericFormat.Hexadecimal:
                return Convert.ToInt64(value, 16);
            default:
                throw Api.Create.Exception("Unexpected Value for base: '{0}'", fromBase);
        }
    }


    /// <summary>
    /// Converts a numeric string into its coressponding UInt32 value.
    /// </summary>
    /// <param name="value">The string on which the extension is invoked.</param>
    /// <param name="fromBase">The base in which the number is represented. Ex (100, NumericFormat.Binary)=>4.</param>
    /// <returns>The numeric value representation of the string.</returns>
    public static UInt32 ToUInt32(this string value, NumericFormat? fromBase = null)
    {
        UpdateFormat(value, ref fromBase);

        switch (fromBase)
        {
            case NumericFormat.Decimal:
                return Convert.ToUInt32(value);
            case NumericFormat.Binary:
                return Convert.ToUInt32(value, 2);
            case NumericFormat.Octal:
                return Convert.ToUInt32(value, 8);
            case NumericFormat.Hexadecimal:
                return Convert.ToUInt32(value, 16);
            default:
                throw Api.Create.Exception("Unexpected Value for base: '{0}'", fromBase);
        }
    }

    /// <summary>
    /// Converts a numeric string into its coresponding double value,
    /// </summary>
    /// <param name="value">String containing the numeric value.</param>
    /// <param name="fromBase">The numeric base in which the string is represented. Only Decimal and Hexadecimal are supported. </param>
    /// <param name="decimalSeparator"></param>
    /// <returns>The numeric value of the string</returns>
    public static double ToDouble(this String value, NumericFormat fromBase = NumericFormat.Decimal, string decimalSeparator = null)
    {
        switch (fromBase)
        {
            case NumericFormat.Decimal:
                return Convert.ToDouble(value.Replace(
                    decimalSeparator ?? ".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));


            //var provider = new NumberFormatInfo();
            //provider.NumberDecimalSeparator = decimalSeparator ?? CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //return Convert.ToDouble(value, provider);
            case NumericFormat.Hexadecimal:
                var b = new byte[value.Length / 2];
                for (int i = (value.Length - 2), j = 0; i >= 0; i -= 2, j++)
                {
                    b[j] = byte.Parse(value.Substring(i, 2), NumberStyles.HexNumber);
                }
                return BitConverter.ToDouble(b, 0);
            default:
                throw Api.Create.Exception("Unexpected value for base: '{0}'", fromBase);
        }
    }

    private static void UpdateFormat(String value, ref NumericFormat? fromBase)
    {
        if (fromBase == null)
        {
            if (value.StartsWith("0x"))
            {
                fromBase = NumericFormat.Hexadecimal;
            }
            else
            {
                fromBase = NumericFormat.Decimal;
            }
        }
    }

    /// <summary>
    /// Convert string to decimal
    /// </summary>
    /// <param name="value">String value</param>
    /// <returns>Decimal value.</returns>
    public static Decimal ToDecimal(this String value)
    {
        return Convert.ToDecimal(value.ToDouble());
    }
    public static FileSystemInfo ToSystemFileInfo(this String fileOrDirectoryFullPath)
    {
        var attr = File.GetAttributes(fileOrDirectoryFullPath);

        if (attr.HasFlag(FileAttributes.Directory))
            return fileOrDirectoryFullPath.ToDirectoryInfo();
        else
            return fileOrDirectoryFullPath.ToFileInfo();
    }

    public static bool IsFile(this FileSystemInfo fileOrFolder)
    {
        return fileOrFolder is FileInfo;
    }

    /// <summary>
    /// Converts the specified path to a file info.
    /// </summary>
    /// <param name="fileFullPath">The full path of the file.</param>
    /// <returns>The file info.</returns>
    public static FileInfo ToFileInfo(this String fileFullPath)
    {
        try
        {
            return new FileInfo(fileFullPath.Trim().Trim('\"'));
        }
        catch (Exception ex)
        {
            throw Api.Create.Exception(ex, $"Unable to create file from path {fileFullPath}");
        }
    }

    /// <summary>
    /// Converts the specified path to a directory info.
    /// </summary>
    /// <param name="directoryFullPath">The full path of the directory.</param>
    /// <returns>The directory info.</returns>
    public static DirectoryInfo ToDirectoryInfo(this string directoryFullPath)
    {
        try
        {
            return new DirectoryInfo(directoryFullPath.Trim().Trim('\"'));
        }
        catch (Exception ex)
        {
            throw Api.Create.Exception(ex, $"Unable to create directory from path '{directoryFullPath}'");
        }
    }


    public static string CommonPrefix(this IEnumerable<string> strings)
    {
        var stringsArray = strings.ToArray();
        if (stringsArray.Length == 0)
        {
            return string.Empty;
        }
        var commonPrefix = new string(
            stringsArray.First().Substring(0, stringsArray.Min(s => s.Length))
                .TakeWhile((c, i) => stringsArray.All(s => s[i] == c)).ToArray());
        return commonPrefix;
    }
    public static string ReplaceLines(this string stringValue, Func<string, string> lineModification,
        string lineBreakString = null)
    {
        var result = stringValue.Split(lineBreakString ?? Environment.NewLine).Select(l => lineModification(l))
            .Where(l => l != null).Aggregate(Environment.NewLine);
        return result;
    }

    /// <summary>
    /// Escapes the xml specific characters in the string.
    /// </summary>
    /// <param name="value">The value that should be placed inside an xml attribute or element.</param>
    /// <returns></returns>
    public static string EscapeXmlCharacters(this string value)
    {
        return SecurityElement.Escape(value);
    }

    /// <summary>
    /// Gets the indices of the specified 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="character"></param>
    /// <returns></returns>
    public static IEnumerable<int> IndicesOf(this String value, char character)
    {
        var startIndex = 0;
        while (true)
        {
            var index = value.IndexOf(character, startIndex);
            if (index == -1)
            {
                yield break;
            }
            else
            {
                yield return index;
                startIndex = index + 1;
            }
        }
    }

    public static string ToNullIfEmpty(this string value)
    {
        if (value.IsNullOrEmpty())
        {
            return null;
        }

        return value;
    }

    /// <summary>
    /// Converts the specified string to camel case.
    /// </summary>
    /// <param name="value">The string value on which the extension is invoked.</param>
    /// <returns>The camel case value.</returns>
    public static String ToPascalCase(this String value)
    {
        if (char.IsLower(value[0]))
        {
            return char.ToUpper(value[0]) + value.Substring(1);
        }

        return value;
    }

 

/// <summary>
    /// Converts the specified string to camel case.
    /// </summary>
    /// <param name="value">The string value on which the extension is invoked.</param>
    /// <returns>The camel case value.</returns>
    public static String ToCamelCase(this String value)
    {
        if (char.IsUpper(value[0]))
        {
            return char.ToLower(value[0]) + value.Substring(1);
        }
        return value;
    }



    /// <summary>
    /// Surrounds the string with the specified pattern
    /// </summary>
    /// <param name="value">The string instance on which the method is called.</param>
    /// <param name="startPattern">The pattern that should be appended at the beginning and end.</param>
    /// <param name="endPattern">The pattern that should be appended at the  end.</param>
    /// <returns>The initial string surrounded with the pattern</returns>
    public static String SurroundWith(this String value, String startPattern, String endPattern = null)
    {
        return String.Format("{0}{1}{2}", startPattern, value, endPattern ?? startPattern);
    }

    /// <summary>
    /// Surrounds the string with quotes
    /// </summary>
    /// <param name="value">The string instance on which the method is called.</param>
    /// <returns>The initial string surrounded with quotes</returns>
    public static String SurroundWithQuotes(this String value)
    {
        return value.SurroundWith("\"");
    }

    /// <summary>
    /// Surrounds the string with quotes
    /// </summary>
    /// <param name="value">The string instance on which the method is called.</param>
    /// <param name="countBefore">Count of newlines befor value.</param>
    /// <param name="countAfter">Count of newlines after value.</param>
    /// <returns>The initial string surrounded with quotes</returns>
    public static String SurroundWithNewline(this String value, int countBefore = 1, int countAfter = 1)
    {
        var newLinesBefore = Environment.NewLine.Repeat(countBefore).ToStringValue();
        var newLinesAfter = Environment.NewLine.Repeat(countAfter).ToStringValue();
        var result = newLinesBefore + value + newLinesAfter;
        return result;
    }

    /// <summary>
    /// Verifies if the string represents a number
    /// </summary>
    /// <param name="value">The string instance on which the method is called.</param>
    /// <returns>True if the string represents a number and false otherwise</returns>
    public static bool IsNumber(this string value)
    {
        //#warning Are you serious???//  return Char.IsNumber(value, 0);
        double dValue;
        return double.TryParse(value, out dValue);
    }


    /// <summary>
    /// Convert Wildcard to regex
    /// </summary>
    /// <param name="pattern">Pattern to convert</param>
    /// <param name="options">Regex option default none</param>
    /// <returns>Regular expression</returns>
    public static Regex WildcardToRegex(this String pattern, RegexOptions options = RegexOptions.None)
    {
        var regexValue = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace(@"\#", "([0-9])") + "$";
        var regex = new Regex(regexValue, options);
        return regex;
    }


    /// <summary>
    /// Compare string by wildcard
    /// </summary>
    /// <param name="value">String to search in</param>
    /// <param name="valueWithWildCard">Search value with wildcard</param>
    /// <param name="ignoreCase">Ignore case value</param>
    /// <param name="ignoreNewLines">Determines whether new lines should be removed for the wildcard comparison.</param>
    /// <returns>Contains wildcard</returns>
    public static bool CompareByWildcard(this String value, String valueWithWildCard, bool ignoreCase = false, bool ignoreNewLines = false)
    {
        if (value == null || valueWithWildCard == null)
        {
            return false;
        }

        if (valueWithWildCard == "*")
        {
            return true;
        }

        if (valueWithWildCard.IndexOf('*') == -1)
        {
            return value.Equals(valueWithWildCard);
        }
        if (ignoreNewLines)
        {
            value = value.Replace("\r", String.Empty).Replace("\n", String.Empty);
        }
        var options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
        var regex = valueWithWildCard.WildcardToRegex(options);
        return regex.IsMatch(value);
    }
    /// <summary>
    /// gets the string between the given indices. index starts from 0.
    /// </summary>
    /// <param name="value">string on which to be used</param>
    /// <param name="startIndex">start index to get the string.</param>
    /// <param name="endIndex">end index to get the string</param>
    /// <returns>string between the given indices.(ie. it excludes the given indices)</returns>
    public static String SubstringBetween(this string value, int startIndex, int endIndex)
    {
        return value.Substring(startIndex, endIndex-startIndex);
    }
    /// <summary>
    /// Gets the string between the given pattern.
    /// </summary>
    /// <param name="value">string on which to be used</param>
    /// <param name="startPattern"></param>
    /// <param name="endPattern"></param>
    /// <returns></returns>
    public static String SubstringBetween(this string value, string startPattern, string endPattern)
    {
        var startIndex = value.IndexOf(startPattern, StringComparison.Ordinal)+startPattern.Length;
        var endIndex = value.IndexOf(endPattern,startIndex, StringComparison.Ordinal);

        return value.SubstringBetween(startIndex, endIndex);
    }

    /// <summary>
    /// Gets the string between the given pattern.
    /// </summary>
    /// <param name="value">string on which to be used</param>
    /// <param name="startPattern"></param>
    /// <param name="endPattern"></param>
    /// <returns></returns>
    public static String SubstringBetween(this string value, char startPattern, char endPattern)
    {
        var startIndex = value.IndexOf(startPattern);
        var endIndex = value.IndexOf(endPattern);

        return value.SubstringBetween(startIndex+1, endIndex);
    }

    /// <summary>
    /// Use an start- and endPattern to cut an substring out
    /// </summary>
    /// <param name="value">String to use on</param>
    /// <param name="startPattern">Start pattern to search on</param>
    /// <param name="includeStartPattern">should the start pattern be used</param>
    /// <param name="endPattern">End pattern to search</param>
    /// <param name="includeEndPattern">Should be use the end pattern</param>
    /// <param name="throwException">Should a exception be thrown.</param>
    /// <returns>Get Substring</returns>
    /// <exception cref="ArgumentException"></exception>
    public static String Substring(this String value, String startPattern = null, bool includeStartPattern = false, String endPattern = null,
        bool includeEndPattern = false, bool throwException = true)
    {
        var startIndex = 0;
        if (startPattern != null)
        {
            var startPatternIndex = value.IndexOf(startPattern, System.StringComparison.Ordinal);
            if (startPatternIndex == -1)
            {
                if (throwException)
                {
                    throw new ArgumentException("The start pattern could not be found.");
                }
            }
            else
            {
                startIndex = startPatternIndex;
            }


            if (!includeStartPattern)
            {
                startIndex += startPattern.Length;
            }
        }

        var endIndex = value.Length - 1;
        if (endPattern != null)
        {
            var endPatternIndex = value.IndexOf(endPattern, startIndex, System.StringComparison.Ordinal);
            if (endPatternIndex == -1)
            {
                if (throwException)
                {
                    throw new ArgumentException("The end pattern could not be found.");
                }
            }
            else
            {
                endIndex = endPatternIndex;
            }

            if (includeEndPattern)
            {
                endIndex += endPattern.Length;
            }
        }

        var result = value.Substring(startIndex, endIndex - startIndex);
        return result;
    }

    /// <summary>
    /// Remove a number of chars at the end
    /// </summary>
    /// <param name="value">String to work on</param>
    /// <param name="count">Number of chars to remove</param>
    /// <returns>Get Substring</returns>
    public static String RemoveLast(this String value, int count)
    {
        return value.Substring(0, value.Length - count);
    }

    /// <summary>
    /// Replaces the format item in a specified <see cref="T:System.String"/> with the text equivalent of the value of a corresponding <see cref="T:System.Object"/> instance in a specified array.
    /// </summary>
    /// <param name="value">The string value that represents the pattern and on which the extension is invoked.</param>
    /// <param name="parameters">The message parameters.</param>
    public static string FormatString(this string value, params object[] parameters)
    {
        return String.Format(value, parameters);
    }

    /// <summary>
    /// Converts the specified base 64 string value back to the original byte array.
    /// </summary>
    /// <param name="base64StringValue">The base64 encoded string value.</param>
    /// <returns>The original byte array that was wrapped in the base 64 string.</returns>
    public static byte[] ToByteArray(this String base64StringValue)
    {
        return Convert.FromBase64String(base64StringValue);
    }

    /// <summary>
    /// Convert an String to Byte Array
    /// </summary>
    /// <param name="value">String value</param>
    /// <returns>String as Byte Array in UTF8</returns>
    public static Byte[] GetBytes(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }
    public static Uri ToUrl(this string uriString, UriKind uriKind)
    {
        var result =  new Uri( uriString,uriKind);
        return result;
    }

    public static Uri ToUrl(this string uriString, Uri baseUrl = null)
    {
        var result = baseUrl == null ? new Uri(uriString) : new Uri(baseUrl, uriString);
        return result;
    }
    public static Uri Combine(this Uri uri, params string[] paths)
    {
        var result = new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        return result;
    }


    /// <summary>
    /// Parse an string with german standard format (DD.MM.YYYY)
    /// </summary>
    /// <param name="dateString">DateTime as string</param>
    /// <param name="format">Default format string</param>
    /// <returns>Parsed DateTime result</returns>
    public static DateTime ToDateTime(this string dateString, string format = "dd.MM.yyyy")
    {
        return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Shrink the string to the given maximum length.
    /// </summary>
    /// <param name="value">The value on which the extension is invoked.</param>
    /// <param name="maximumLength">The maximum length of the returned string.</param>
    /// <param name="atTheBeginning"></param>
    /// <param name="addShrinkPattern">Determines whether 3 dots should be attached to indicate that the string has been shrinked.</param>
    /// <returns>The shrink string.</returns>
    public static string Shrink(this String value, int maximumLength, bool atTheBeginning = false, bool addShrinkPattern = false)
    {
        if (value.Length < maximumLength)
        {
            return value;
        }

        var shrinkedPattern = addShrinkPattern ? "..." : string.Empty;
        var result =  atTheBeginning
            ? shrinkedPattern + value.Substring(value.Length-maximumLength)
            : value.Substring(0, maximumLength) + shrinkedPattern;
        return result;
    }

    /// <summary>
    /// Escapes the string so that it can be used for string format without any problems.
    /// </summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value</returns>
    public static string EscapeForFormat(this String value)
    {
        return value.Replace("{", "{{").Replace("}", "}}");
    }

    /// <summary>
    /// Parse a string to correct sting
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <param name="template">String that are the template</param>
    /// <returns>Array of parse strings</returns>
    public static string[] Parse(this String value, String template)
    {
        template = Regex.Replace(template, @"[\\\^\$\.\|\?\*\+\(\)]", m => "\\" + m.Value);

        var pattern = "^" + template.Replace("\\*", "(.*?)") + "$";

        var regex = new Regex(pattern);
        var match = regex.Match(value);

        return match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();


    }

    /// <summary>
    /// Adds a new line to the given string.
    /// </summary>
    /// <param name="value">The string on which the extension is invoked.</param>
    /// <returns>The string plus new line.</returns>
    [Obsolete("Use SurroundWithNewLine(0) instead.")]
    public static string AddNewLine(this String value)
    {
        return value + Environment.NewLine;
    }

    /// <summary>
    /// Splits a string according to the string tyoe separators received as parammeters.
    /// </summary>
    /// <param name="value">The strings to be split</param>
    /// <param name="seperators">The separators used for the splitting</param>
    /// <returns>Array containing the split elements</returns>
    public static string[] Split(this String value, params string[] seperators)
    {
        return value.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
    }


    /// <summary>
    /// Indicates whether the specified <see cref="String"/> instance is null or empty.
    /// </summary>
    /// <param name="instance">The string that invokes the extension method.</param>
    /// <returns>true, if the string is null or empty, otherwise false.</returns>
    public static bool IsNullOrEmpty(this String instance)
    {
        return String.IsNullOrEmpty(instance);
    }

    /// <summary>
    /// Tries to convert the string to an double value. If the value does not represent an double, null is returned.
    /// </summary>
    /// <param name="value">The string instance on which this extension is invoked.</param>
    /// <param name="formatCulture">Culture.</param>
    /// <returns>The double value or null.</returns>
    public static double? TryToDouble(this String value, IFormatProvider formatCulture = null)
    {
        double result;
        var parsable = double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, 
            formatCulture ?? NumberFormatInfo.CurrentInfo, out result);

        return parsable ? result : default(double?);
    }

    /// <summary>
    /// Tries to convert the string to an int value. If the value does not represent an int, null is returned.
    /// </summary>
    /// <param name="value">The string instance on which this extension is invoked.</param>
    /// <returns>The int value or null.</returns>
    public static int? TryToInt32(this String value)
    {
        int result;
        return int.TryParse(value, out result) ? result : default(int?);
    }

    /// <summary>
    /// Tries to convert the string to an bool value. If the value does not represent an int, the default bool value is returned.
    /// </summary>
    /// <param name="value">The string instance on which this extension is invoked.</param>
    /// <returns>The bool value or default of bool.</returns>
    public static bool? TryToBoolean(this String value)
    {
        var intValue = value.TryToInt32();
        if (intValue != null)
        {
            return Convert.ToBoolean(intValue.Value);
        }
        bool boolParseResult;
        if (bool.TryParse(value, out boolParseResult))
        {
            return boolParseResult;
        }
        return null;
    }


    /// <summary>
    /// Check if a string value can parse
    /// </summary>
    /// <param name="value">String value to pars</param>
    /// <returns></returns>
    public static bool IsInt32(this String value)
    {
        int parseValue;
        return Int32.TryParse(value, out parseValue);
    }

    /// <summary>
    /// Creates a Guid from a string.
    /// </summary>
    /// <param name="value">The string instance on which this extension is invoked.</param>
    /// <param name="fromMd5Hash">Use an MD5 algorithm or not</param>
    /// <returns>Guid created from the string</returns>
    public static Guid CreateGuid(this String value, bool fromMd5Hash = true)
    {
        if (fromMd5Hash)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(value));
                var result = new Guid(hash);
                return result;
            }

        }
        else
        {
            var hashCode = value.GetHashCode();
            var hash8 = hashCode.ToString("X8");
            var hash4 = hash8.Remove(4);

            var guidString = String.Format("{0}-{1}-{1}-{1}-{0}{1}", hash8, hash4);
            return new Guid(guidString);
        }
    }





    /// <summary>
    /// Replaces the character of the string at the specified index with the specified character.
    /// </summary>
    /// <param name="value">The string on which the extension method is invoked.</param>
    /// <param name="index">The index at which the character should be replaced.</param>
    /// <param name="character">The character that should be inserted.</param>
    /// <returns>The string with the replaced character.</returns>
    public static String ReplaceAt(this String value, int index, char character)
    {
        var array = value.ToCharArray();
        array[index] = character;
        return new string(array);
    }

    /// <summary>
    /// Returns false if the string equals "0" and true otherwise.
    /// </summary>
    /// <param name="value">The input value</param>
    /// <returns> False for "0" and true otherwise. </returns>
    public static bool ToBool(this String value)
    {
        switch (value)
        {
            default:
                throw Api.Create.Exception(
                    "The string '{0}' could not be converted to a boolean value", value);
            case "0":
            case "False":
            case "false":
            case "FALSE":
                return false;
            case "1":
            case "True":
            case "true":
            case "TRUE":
                return true;
        }
    }

   
    /// <summary>
    /// Gets whether the string contains any special characters. Returns false if only numbers or letters are used.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>Whether the string contains any special characters.</returns>
    public static bool HasSpecialCharacters(this string value)
    {
        return value.Any(ch => !char.IsLetterOrDigit(ch));
    }

    /// <summary>
    /// Get substring by wildcard sting with ONE wildcard char
    /// </summary>
    /// <param name="value">Input string value.</param>
    /// <param name="searchPattern">Search pattern string.</param>
    /// <param name="includePattern">Should pattern be included in substring.</param>
    /// <returns></returns>
    public static IEnumerable<string> SubstringsByWildcard(this string value, string searchPattern, bool includePattern = false)
    {
        var indices = IndicesByWildcard(value, searchPattern);
        var substringPattern = searchPattern.Split("*");
        return indices.Select(index => value.Substring(index)
            .Substring(substringPattern[0], includePattern, substringPattern[1], includePattern)).ToArray();
    }

    /// <summary>
    /// Get indices by wildcard
    /// </summary>
    /// <param name="value">Input string.</param>
    /// <param name="searchPattern">Wildcard pattern string.</param>
    /// <returns>Indices.</returns>
    public static IEnumerable<int> IndicesByWildcard(this string value, string searchPattern)
    {
        var indices = new List<int>();
        foreach (Match match in Regex.Matches(value, searchPattern.Split('*').First()))
        {
            indices.Add(match.Index);
        }
        return indices.ToArray();
    }
}


