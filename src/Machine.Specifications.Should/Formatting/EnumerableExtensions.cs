using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Formatting
{
    internal static class EnumerableExtensions
    {
        public static string Format<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            var arrayValues = array.Select(x => x.Format().Indent())
                .Take(10)
                .ToArray();

            var result = new StringBuilder()
                .AppendLine("{")
                .Append(string.Join(",\r\n", arrayValues));

            if (array.Length > 10)
            {
                if (array.Length > 11)
                {
                    result.AppendLine($",\r\n  ...({array.Length - 10} more elements)");
                }
                else
                {
                    result.AppendLine(",\r\n" + array.Last().Format().Indent());
                }
            }
            else
            {
                result.AppendLine();
            }

            return result.AppendLine("}").ToString();
        }
    }
}
