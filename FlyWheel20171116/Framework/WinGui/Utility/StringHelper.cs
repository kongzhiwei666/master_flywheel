using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.WinGui.Utility
{
    /// <summary>
    /// Summary description for StringHelper.
    /// </summary>
    public sealed class StringHelper
    {

        // 'The next line is supposed to be an RFC 2822 address compliant validation expression
        private static Regex regexEMail = new Regex(@"(?<prefix>mailto:)?(?<address>(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$)", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        /// <summary>
        /// Helper to test strings.
        /// </summary>
        /// <param name="text">String to test</param>
        /// <returns>True, if 'text' was null of of length zero</returns>
        public static bool EmptyOrNull(string text)
        {
            return (text == null || text.Trim().Length == 0);
        }

        /// <summary>
        /// Cuts the provided string at allowedLength - 3 (ellipsis length)
        /// </summary>
        /// <param name="text">string to work on</param>
        /// <param name="allowedLength">Max. length of the string to return</param>
        /// <returns>string that ends with ellipsis (...)</returns>
        /// <remarks>Considers newline and linefeed</remarks>
        public static string ShortenByEllipsis(string text, int allowedLength)
        {
            if (text == null) return String.Empty;

            if (text.Length > allowedLength + 3)
            {
                int nlPos = text.IndexOfAny(new char[] { '\n', '\r' });
                if (nlPos >= 0 && nlPos < allowedLength)
                    return text.Substring(0, nlPos) + "...";
                else
                    return text.Substring(0, allowedLength) + "...";
            }
            else
                return text;
        }

        /// <summary>
        /// Return the first amount of words defined by 'wordCount' contained in 'text'.
        /// </summary>
        /// <param name="text">String to work on</param>
        /// <param name="wordCount">Amount of words to look for</param>
        /// <returns>String containing only the first x words.</returns>
        /// <remarks>Word delimiters are: linefeed, carrige return, tab and space</remarks>
        public static string GetFirstWords(string text, int wordCount)
        {
            if (text == null) return String.Empty;
            string[] a = text.Split(new char[] { '\n', '\r', '\t', ' ' });
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Math.Min(a.Length, wordCount); i++)
            {
                sb.Append(a[i]);
                sb.Append(" ");
            }
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Tests the <c>text</c> for a valid e-Mail address and returns true,
        /// if the content match, else false. 
        /// </summary>
        /// <param name="text">String to test</param>
        /// <returns>True if it looks like a valid e-Mail address, else false.</returns>
        public static bool IsEMailAddress(string text)
        {
            if (EmptyOrNull(text))
                return false;
            return regexEMail.IsMatch(text.Trim());
        }

        public static string GetEMailAddress(string text)
        {
            if (EmptyOrNull(text))
                return String.Empty;

            Match m = regexEMail.Match(text);
            if (m.Success)
            {
                return m.Groups["address"].Value;
            }
            else
            {
                return text;
            }
        }

        private StringHelper() { }
    }

}
