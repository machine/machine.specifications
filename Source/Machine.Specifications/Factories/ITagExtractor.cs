using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;
using System.Reflection;

namespace Machine.Specifications.Factories
{
    internal interface ITagExtractor
    {
        IEnumerable<Tag> ExtractTags(Type type);
    }

    internal class HybridTagExtractor : ITagExtractor
    {
        public IEnumerable<Tag> ExtractTags(Type type)
        {
            var interfaceTagExtractor = new InterfaceTagExtractor();
            var attributeTagExtractor = new AttributeTagExtractor();

            var tags = interfaceTagExtractor.ExtractTags(type).Union(attributeTagExtractor.ExtractTags(type)).Distinct();

            return tags.ToList();
        }
    }

    internal class InterfaceTagExtractor : ITagExtractor
    {
        public IEnumerable<Tag> ExtractTags(Type type)
        {
            var tags = type.GetTypeInfo().GetInterfaces().Where(x => x.FullName.StartsWith("Machine.Specifications.Tags`"))
              .SelectMany(x => x.GetGenericArguments())
              .Select(x => new Tag(x.Name)).Distinct();

            return tags.ToList();
        }
    }

    internal class AttributeTagExtractor : ITagExtractor
    {
        public IEnumerable<Tag> ExtractTags(Type type)
        {
            var tags = type.GetTypeInfo().GetCustomAttributes(typeof(TagsAttribute), true).SelectMany(x => ((TagsAttribute)x).Tags).Distinct();

            return tags.ToList();
        }
    }
}
