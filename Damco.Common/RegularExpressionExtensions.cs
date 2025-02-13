using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class RegularExpressionExtensions
    {
        //public static string GetValueOrDefault(this Match match, string groupName)
        //{
        //    if (match == null || match.g [groupName])
        //        return null;
        //    else
        //        return match.Groups[groupName];
        //}

        public static IEnumerable<string> GetMatchingTexts(this Regex regex, string input)
        {
            return regex
                .Matches(input).Cast<Match>()
                .Select(x => x.Value)
                .Distinct();
        }
    }
}
