using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Yal
{
    static class MyListExtensions
    {
        public static StringCollection ToStringCollection(this IList<string> list)
        {
            var sc = new StringCollection();
            sc.AddRange(list.ToArray());
            return sc;
        }
    }
}
