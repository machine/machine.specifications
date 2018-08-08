using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class ContextFilter
    {
        public ContextFilter(string name, IEnumerable<SpecifiactionFilter> specificFilters)
        {
            Name = name;
            SpecificationFilters = specificFilters.ToList();
        }

        public string Name { get; }

        public IEnumerable<SpecifiactionFilter> SpecificationFilters { get; }
    }
}