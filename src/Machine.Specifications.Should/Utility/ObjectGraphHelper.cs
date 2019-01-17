using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            if (objectType.IsArray || (obj is IEnumerable && objectType != typeof(string)))
                return GetSequenceNode(obj);

            if (objectType.GetTypeInfo().IsClass && objectType != typeof(string))
                return GetKeyValueNode(obj);

            return new LiteralNode { Value = obj };
        }

        static INode GetSequenceNode(object obj)
        {
            var sequence = ((IEnumerable)obj).Cast<object>();

            return new SequenceNode { ValueGetters = sequence.Select<object, Func<object>>(a => () => a).ToArray() };
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

            return new KeyValueNode { KeyValues = properties.Concat(fields).OrderBy(m => m.Name) };
        }

        public class SequenceNode : INode
        {
            public IEnumerable<Func<object>> ValueGetters { get; set; }
        }

        public interface INode
        {
        }

        public class KeyValueNode : INode
        {
            public IEnumerable<Member> KeyValues { get; set; }
        }

        public class LiteralNode : INode
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