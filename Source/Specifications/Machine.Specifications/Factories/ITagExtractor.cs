using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
  public interface ITagExtractor
  {
    IEnumerable<Tag> ExtractTags(Type type);
  }

  public class HybridTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var interfaceTagExtractor = new InterfaceTagExtractor();
      var attributeTagExtractor = new AttributeTagExtractor();

      var tags = interfaceTagExtractor.ExtractTags(type).Union(attributeTagExtractor.ExtractTags(type)).Distinct();

      return tags.ToList();
    }
  }

  public class InterfaceTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var tags = type.GetInterfaces().Where(x => x.FullName.StartsWith("Machine.Specifications.Tags`"))
        .SelectMany(x => x.GetGenericArguments())
        .Select(x => new Tag(x.Name)).Distinct();

      return tags.ToList();
    }
  }

  public class AttributeTagExtractor : ITagExtractor
  {
    public IEnumerable<Tag> ExtractTags(Type type)
    {
      var tags = type.GetCustomAttributes(typeof(TagsAttribute), true).SelectMany(x => ((TagsAttribute)x).Tags).Distinct();

      return tags.ToList();
    }
  }
}
