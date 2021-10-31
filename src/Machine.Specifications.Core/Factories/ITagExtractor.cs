using System;
using System.Collections.Generic;
using Machine.Specifications.Model;

namespace Machine.Specifications.Factories
{
    internal interface ITagExtractor
    {
        IEnumerable<Tag> ExtractTags(Type type);
    }
}
