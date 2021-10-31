using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Runner;

namespace Machine.Specifications.Explorers
{
    public static class FilteringExtensions
    {
        public static IEnumerable<Type> FilterBy(this IEnumerable<Type> types, RunOptions options)
        {
            if (options == null)
            {
                return types;
            }

            var filteredTypes = types;

            var restrictToTypes = new HashSet<string>(options.Filters, StringComparer.OrdinalIgnoreCase);

            if (restrictToTypes.Any())
            {
                filteredTypes = filteredTypes.Where(x => restrictToTypes.Contains(x.FullName));
            }

            var includeTags = new HashSet<Tag>(options.IncludeTags.Select(tag => new Tag(tag)));
            var excludeTags = new HashSet<Tag>(options.ExcludeTags.Select(tag => new Tag(tag)));

            if (includeTags.Any() || excludeTags.Any())
            {
                var extractor = new AttributeTagExtractor();

                var filteredTypesWithTags = filteredTypes.Select(type => (Type: type, Tags: extractor.ExtractTags(type)));

                if (includeTags.Any())
                {
                    filteredTypesWithTags = filteredTypesWithTags.Where(x => x.Tags.Intersect(includeTags).Any());
                }

                if (excludeTags.Any())
                {
                    filteredTypesWithTags = filteredTypesWithTags.Where(x => !x.Tags.Intersect(excludeTags).Any());
                }

                filteredTypes = filteredTypesWithTags.Select(x => x.Type);
            }

            return filteredTypes;
        }
    }
}
