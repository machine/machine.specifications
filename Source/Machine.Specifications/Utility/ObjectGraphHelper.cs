using System;
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
            if(obj.GetType().IsArray)
                return GetArrayNode(obj);
            else
                return GetKeyValueNode(obj);
        }

        static INode GetArrayNode(object obj)
        {
            return new ArrayNode();
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

        class ArrayNode : INode
        {
        }

        public interface INode
        {
        }

        public class KeyValueNode : INode
        {
            public IOrderedEnumerable<Member> KeyValues { get; set; }
        }

        public class Member
        {
            public string Name { get; set; }

            public Func<object> ValueGetter { get; set; }
        }
    }
}