using System.Collections;
using System.Linq;

namespace Machine.Specifications.Formatting
{
    internal static class ObjectFormatter
    {
        public static string Format(object value)
        {
            if (value == null)
            {
                return "[null]";
            }

            if (value is string valueAsString)
            {
                return $@"""{valueAsString.Replace("\n", "\\n")}""";
            }

            var type = value.GetType();

            if (type.IsValueType)
            {
                return "[" + value + "]";
            }

            if (value is IEnumerable items)
            {
                var enumerable = items.Cast<object>();

                return type + ":\r\n" + enumerable.EachToUsefulString();
            }

            var stringValue = value.ToString();

            if (stringValue == null || stringValue.Trim() == string.Empty)
            {
                return $"{type}:[]";
            }

            stringValue = stringValue.Trim();

            if (stringValue.Contains("\n"))
            {
                return string.Format(@"{1}:
[
{0}
]", stringValue.Indent(), type);
            }

            var typeString = type.ToString();

            if (typeString == stringValue)
            {
                return typeString;
            }

            return $"{type}:[{stringValue}]";
        }
    }
}
