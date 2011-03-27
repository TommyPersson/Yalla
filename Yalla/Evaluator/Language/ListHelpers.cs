using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yalla.Evaluator.Language
{
    public class ListHelpers
    {        
        public static IEnumerable<object> Rest(IEnumerable lst)
        {
            return lst.OfType<object>().Skip(1);
        }

        public static bool IsEmpty(IEnumerable lst)
        {
            return !lst.OfType<object>().Any();
        }

        public static object First(IEnumerable lst)
        {
            return lst.OfType<object>().FirstOrDefault();
        }

        public static object FirstOrDefault(IEnumerable lst, Func<object, bool> pred)
        {
            return lst.OfType<object>().FirstOrDefault(pred);
        }

        public static object Filter(IEnumerable lst, Func<object, bool> pred)
        {
            return lst.OfType<object>().Where(pred);
        }
        
        public static object OfType(Type type, IEnumerable lst)
        {
            var ofTypeMethodInfo = typeof(Enumerable).GetMethod("OfType");
            var genericMeth = ofTypeMethodInfo.MakeGenericMethod(type);
            var result = genericMeth.Invoke(null, new[] { lst });
            
            return result;
        }
    }
}
