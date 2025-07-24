using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Webapi.BasicToolRepo.Logging
{
    [ExcludeFromCodeCoverage]
    public static partial class Sanitizer
    {
        [GeneratedRegex("<.*?>")]
        private static partial Regex HtmlTagRegex();
        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            // Example: Remove HTML tags                     
            string sanitized = HtmlTagRegex().Replace(input, string.Empty);
            // Example: Escape special characters
            sanitized = sanitized.Replace("'", "''");
            sanitized = sanitized.Replace("\"", "\\\"");
            return sanitized;
        }
    }
}
