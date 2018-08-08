using System;

namespace Machine.Specifications.Runner
{
#if !NETSTANDARD
    [Serializable]
#endif
    public class SpecifiactionFilter
    {
        public SpecifiactionFilter(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}