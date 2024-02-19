using System;
using System.Text.RegularExpressions;
using System.Web;

public static class StringExtensions
{
    // Accesses a character at the given position.
    private static char CharAt(this string str, int offset)
    {
        return str[offset];
    }

    // Returns a substring from the given start index to the given end index.
    public static string Substring(this string str, int from, int to)
    {
        if (from >= str.Length || to >= str.Length || to - from < 0)
            return null;

        return str[from..(to + 1)];
    }

    // Returns a substring from the given start index to the end of the string.
    public static string Substring(this string str, int from)
    {
        return str[from..];
    }

    // Returns a substring of the specified length from the given start index.
    public static string Substring(this string str, int from, int length)
    {
        return str.Substring(from, from + length - 1);
    }

    // Returns a substring up to the specified end index.
    public static string SubstringTo(this string str, int to)
    {
        return str[..to];
    }

    // Removes substrings that start and end with specific strings.
    public static string RemoveOccurrencesOfSubstring(this string str, string start, string end)
    {
        var copy = str;
        var pattern = $"{Regex.Escape(start)}[^>]+{Regex.Escape(end)}";
        var regex = new Regex(pattern);
        while (true)
        {
            var match = regex.Match(copy);
            if (!match.Success)
                break;
            copy = copy.Remove(match.Index, match.Length);
        }
        return copy;
    }

    // Replaces characters in the string based on a character set.
    public static string Replace(this string str, char[] oldChars, char newChar)
    {
        foreach (var c in oldChars)
        {
            str = str.Replace(c, newChar);
        }
        return str;
    }

    // Replaces every digit in the string with its spelled out word.
    public static string SpelledOutDigits(this string str, System.Globalization.CultureInfo cultureInfo = null)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        var output = string.Empty;

        foreach (var c in str)
        {
            if (char.IsDigit(c))
            {
                var spelledOutChar = int.Parse(c.ToString()).ToWords(cultureInfo) + " ";
                output += spelledOutChar;
            }
            else
            {
                output += c;
            }
        }

        return output;
    }

    // Returns the accessibility version of the string.
    public static string AccessibilityString(this string str)
    {
        return str.ToLower().Replace("callout", "call out");
    }

    // Encodes the string for use in a URL.
    public static string UrlEncoded(this string str, bool plusForSpace = true)
    {
        var allowed = new System.Text.StringBuilder();
        allowed.Append(System.Web.HttpUtility.UrlEncode("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._"));
        if (plusForSpace)
        {
            allowed.Append("+");
        }
        
        var encoded = HttpUtility.UrlEncode(str, allowed.ToString());
        
        if (plusForSpace)
        {
            encoded = encoded.Replace(" ", "+");
        }

        return encoded;
    }

    // Checks if the string represents a valid geocoordinate.
    public static bool IsGeocoordinate(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        var parts = str.Split('(', ')', ',');

        if (parts.Length != 2)
            return false;

        if (!double.TryParse(parts[0], out double latitude) || !double.TryParse(parts[1], out double longitude))
            return false;

        return true; // Assuming the coordinate format is correct
    }

    // Returns the number of string arguments ("%@" or "%n$@").
    private static int ArgumentCount(this string str)
    {
        var pattern = @"%(@|\d+\$@)";
        var regex = new Regex(pattern);
        var matches = regex.Matches(str);
        return matches.Count;
    }

    // Initializes a string object with a format and arguments.
    public static string NormalizedArgsWithFormat(this string str, string format, string[] arguments)
    {
        var formatArgCount = format.ArgumentCount();

        if (formatArgCount == arguments.Length)
        {
            return string.Format(format, arguments);
        }

        var preciseArgs = new string[formatArgCount];
        if (formatArgCount < arguments.Length)
        {
            Array.Copy(arguments, preciseArgs, formatArgCount);
        }
        else
        {
            Array.Copy(arguments, preciseArgs, arguments.Length);
            for (int i = arguments.Length; i < formatArgCount; i++)
            {
                preciseArgs[i] = "(null)";
            }
        }

        Console.WriteLine($"String format warning: \"{format}\" has {formatArgCount} arguments but was passed {arguments.Length}");

        return string.Format(format, preciseArgs);
    }
}
