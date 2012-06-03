using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Utility
{
    /// <summary>
    /// Modified from Steve Wagner's ObjectDiff prototype https://gist.github.com/2841822
    /// </summary>
    public static class ObjectGraphHelper
    {
        public static INode GetGraph(object obj)
        {
            var objectType = obj.GetType();
            if (objectType.IsArray)
            {
                return GetArrayNode(obj);
            }
            else if(objectType.IsClass && objectType != typeof(string))
            {
                return GetKeyValueNode(obj);
            }
            else
            {
                return new LiteralNode {Value = obj};
            }
        }

        static INode GetArrayNode(object obj)
        {
            var array = (IEnumerable<object>) obj;

            return new ArrayNode {ValueGetters = array.Select<object, Func<object>>(a => () => a).ToArray()};
        }

        static INode GetKeyValueNode(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.CanRead)
                .Select(p =>
                {
                    var prop = p;
                    return new Member
                    {
                        Name = p.Name,
                        ValueGetter = () => prop.GetValue(obj, null)
                    };
                });

            var fields = obj.GetType().GetFields()
                .Select(f =>
                {
                    var field = f;
                    return new Member
                    {
                        Name = f.Name,
                        ValueGetter = () => field.GetValue(obj)
                    };
                });

            return new KeyValueNode {KeyValues = properties.Concat(fields).OrderBy(m => m.Name)};
        }

        public interface INode
        {
        }

        public class ArrayNode : INode
        {
            public IEnumerable<Func<object>> ValueGetters { get; set; }
        }

        public class KeyValueNode : INode
        {
            public IEnumerable<Member> KeyValues { get; set; }
        }

        public class LiteralNode:INode
        {
            public object Value { get; set; }
        }

        public class Member
        {
            public string Name { get; set; }

            public Func<object> ValueGetter { get; set; }
        }
    }
}