using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Model;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TagsAttribute : Attribute
    {
        private readonly List<Tag> tags;

        public TagsAttribute(string tag, params string[] additionalTags)
        {
            tags = new List<Tag> {new Tag(tag)};
            tags.AddRange(additionalTags.Select(x => new Tag(x)));
        }

        public IEnumerable<Tag> Tags => tags;
    }
}
