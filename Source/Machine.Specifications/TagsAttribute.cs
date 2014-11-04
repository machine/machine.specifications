using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TagsAttribute : Attribute
    {
        readonly List<Tag> _tags;

        public IEnumerable<Tag> Tags { get { return _tags; } }

        public TagsAttribute(string tag, params string[] additionalTags)
        {
            _tags = new List<Tag>();
            _tags.Add(new Tag(tag));
            _tags.AddRange(additionalTags.Select(x => new Tag(x)));
        }
    }
}
