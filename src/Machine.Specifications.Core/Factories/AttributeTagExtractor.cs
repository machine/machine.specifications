using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
    internal class AttributeTagExtractor : ITagExtractor
    {
        public IEnumerable<Tag> ExtractTags(Type type)
        {
            var tags = type.GetTypeInfo().GetCustomAttributes(typeof(TagsAttribute), true).SelectMany(x => ((TagsAttribute)x).Tags).Distinct();

            return tags.ToList();
        }
    }
}
