using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);

        public static string SafeTrim(this string s) => s.IsNullOrWhiteSpace() ? string.Empty : s.Trim();

        public static string ToSlug(this string s) => Regex.Replace(s, "(?<!^)([A-Z])", m => $"-{m.Value}").ToLower();

        public static string IfNullOrWhiteSpaceWhen(this string s, string whenString) => s.IsNullOrWhiteSpace() ? whenString : s;

        public static string EnsureEndsWith(this string s, char c)
        {
            if (s == null) return c.ToString();

            return s.EndsWith(c) ? s : s + c;
        }

        public static T ToEnum<T>(this string s)
        {
            return Enum.TryParse(typeof(T), s, true, out var v) ? (T)v : default(T);
        }

        public static string SafeCut(this string s, int length)
        {
            if (s.IsNullOrWhiteSpace() || s.Length <= length) return s;

            return s.Substring(0, length);
        }

        public static string ToTitleCase(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        }

        /// <summary>
        /// Capitalises the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string Capitalize(this string input)
        {
            try
            {
                if (String.IsNullOrEmpty(input))
                    return String.Empty;

                if (input.Length > 1)
                {
                    var splitted = input.Split(' ');
                    if (splitted.Length > 0)
                    {
                        return string.Join(" ", splitted.Select(s => Char.ToUpper(s[0]) + s.Substring(1).ToLower()));
                    }

                    return Char.ToUpper(input[0]) + input.Substring(1).ToLower();
                }

                return Char.ToUpper(input[0]).ToString();
            }
            catch
            {
                return Char.ToUpper(input[0]).ToString();
            }
        }
    }
}
