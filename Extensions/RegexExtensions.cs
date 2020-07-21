using System;
using System.Text.RegularExpressions;

namespace redseat_server.Extensions
{
    public static class RegexExtensions
    {
        public static int EndIndex(this Capture regexMatch)
        {
            return regexMatch.Index + regexMatch.Length;
        }
    }
}