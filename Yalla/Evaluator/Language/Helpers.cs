using System.Collections.Generic;
using System.Linq;

namespace Yalla.Evaluator.Language
{
    public class Helpers
    {
        public static string Str(IList<object> strings)
        {
            return string.Concat(strings.Select(x => x.ToString()));
        }
    }
}
