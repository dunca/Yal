using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YalBookmark
{
    static class MyStringExtensions
    {
        public static bool ContainsFuzzy(this string str, string pattern, bool matchAnywhere)
        {
            int lastMatch = -1;
            str = str.ToLower();
            pattern = str.ToLower();
            foreach (char c in pattern)
            {
                lastMatch = str.IndexOf(c, lastMatch);
                if (lastMatch == -1)
                {
                    return false;
                }
            }
            return matchAnywhere ? true : str[0] == pattern[0];
        }

        public static bool ContainsWithCondition(this string str, string pattern, bool matchAnywhere)
        {
            return matchAnywhere ? str.IndexOf(pattern, StringComparison.CurrentCultureIgnoreCase) > -1 : 
                                   str.ToLower().StartsWith(pattern.ToLower());
        }
    }
}
