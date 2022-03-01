using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications
{
    public static class ShouldLikeExtensions
    {
        public static void ShouldBeSimilar(this object value, object expected)
        {

        }

        private static INode GetNode(object value)
        {
            var type = value.GetType();

            return value switch
            {
                IEnumerable enumerable when type != typeof(string) => new ArrayNode(enumerable),
                not null when type.IsClass && type != typeof(string) => new ObjectNode(value),
                _ => new ValueNode(value)
            };
        }

        private class Member
        {
            public Member(string name, Func<object> value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; }

            public Func<object> Value { get; }
        }

        private interface INode
        {
            IEnumerable<SpecificationException> CompareTo(INode node);
        }

        private class ValueNode : INode
        {
            private readonly object? value;

            public ValueNode(object? value)
            {
                this.value = value;
            }

            public IEnumerable<SpecificationException> CompareTo(INode node)
            {
                throw new NotImplementedException();
            }
        }

        private class ObjectNode : INode
        {
            private readonly Member[] members;

            public ObjectNode(object value)
            {
                var type = value.GetType();

                var properties = type
                    .GetProperties()
                    .Where(x => x.CanRead && !x.GetGetMethod().IsStatic)
                    .Select(x => new Member(x.Name, () => x.GetValue(value, null)));

                var fields = type
                    .GetFields()
                    .Select(x => new Member(x.Name, () => x.GetValue(value)));

                members = properties
                    .Concat(fields)
                    .OrderBy(m => m.Name)
                    .ToArray();
            }
        }

        private class ArrayNode : INode
        {
            private readonly Func<object>[] getters;

            public ArrayNode(IEnumerable enumerable)
            {
                getters = enumerable
                    .Cast<object>()
                    .Select<object, Func<object>>(x => () => x)
                    .ToArray();
            }
        }
    }
}
