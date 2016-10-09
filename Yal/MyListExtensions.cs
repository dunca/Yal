using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Yal
{
    static class MyIEnumerableExtensions
    {
        public static StringCollection ToStringCollection(this IEnumerable<string> list)
        {
            var sc = new StringCollection();
            sc.AddRange(list.ToArray());
            return sc;
        }
    }
}
