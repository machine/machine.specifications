using System.Collections;
using System.Linq;
using System.Reflection;

namespace Machine.Specifications.Text
{
    internal static class ObjectExtensions
    {
        internal static string ToUsefulString(this object value)
        {
            if (value == null)
            {
                return "[null]";
            }

            if (value is string stringValue)
            {
                return "\"" + stringValue.Replace("\n", "\\n") + "\"";
            }

            if (value.GetType().GetTypeInfo().IsValueType)
            {
                return "[" + value + "]";
            }

            if (value is IEnumerable items)
            {
                var enumerable = items.Cast<object>();

                return items.GetType() + ":\r\n" + enumerable.EachToUsefulString();
            }

            var str = value.ToString();

            if (str == null || str.Trim() == string.Empty)
            {
                return $"{value.GetType()}:[]";
            }

            str = str.Trim();

            if (str.Contains("\n"))
            {
                return string.Format(@"{1}:
[
{0}
]", str.Indent(), value.GetType());
            }

            if (value.GetType().ToString() == str)
            {
                return value.GetType().ToString();
            }

            return $"{value.GetType()}:[{str}]";
        }
    }
}
