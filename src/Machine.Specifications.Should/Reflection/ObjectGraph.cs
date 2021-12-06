using System;
using System.Collections;
using System.Linq;

namespace Machine.Specifications.Reflection
{
    internal static class ObjectGraph
    {
        public static INode Get(object value)
        {
            var type = value.GetType();

            if (type.IsArray || value is IEnumerable && type != typeof(string))
            {
                return GetSequenceNode((IEnumerable) value);
            }

            if (type.IsClass && type != typeof(string))
            {
                return GetKeyValueNode(value);
            }

            return new LiteralNode
            {
                Value = value
            };
        }

        private static INode GetSequenceNode(IEnumerable value)
        {
            var getters = value.Cast<object>()
                .Select<object, Func<object>>(x => () => x)
                .ToArray();

            return new SequenceNode
            {
                ValueGetters = getters
            };
        }

        private static INode GetKeyValueNode(object value)
        {
            var type = value.GetType();

            var properties = type
                .GetProperties()
                .Where(x => x.CanRead && !x.GetGetMethod().IsStatic)
                .Select(x =>
                {
                    return new Member
                    {
                        Name = x.Name,
                        ValueGetter = () => x.GetValue(value, null)
                    };
                });

            var fields = type
                .GetFields()
                .Select(x =>
                {
                    return new Member
                    {
                        Name = x.Name,
                        ValueGetter = () => x.GetValue(value)
                    };
                });

            return new KeyValueNode
            {
                KeyValues = properties.Concat(fields).OrderBy(m => m.Name)
            };
        }
    }
}
