using System;
using System.Collections.Generic;
using System.Linq;

namespace Yalla.Evaluator.Language
{
    public class ListHelpers
    {
        public static IEnumerable<object> Rest(IEnumerable<object> lst)
        {
            return lst.Skip(1);
        }

        public static bool IsEmpty(IEnumerable<object> lst)
        {
            using (var e = lst.GetEnumerator())
            {
                return !e.MoveNext();
            }
        }

        public static object First(IEnumerable<object> lst)
        {
            return lst.First();
        }

        public static object FirstOrDefault(IEnumerable<object> lst, Func<object, bool> pred)
        {
            return lst.FirstOrDefault(pred);
        }
    }
}
