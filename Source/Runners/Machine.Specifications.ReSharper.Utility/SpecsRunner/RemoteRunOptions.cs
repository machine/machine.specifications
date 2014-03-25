namespace Machine.Specifications.Runner.Utility.SpecsRunner
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RemoteRunOptions
    {
        public IEnumerable<string> IncludeTags { get; private set; }
        public IEnumerable<string> ExcludeTags { get; private set; }
        public IEnumerable<string> Filters { get; private set; }

        public RemoteRunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters)
        {
            this.IncludeTags = includeTags;
            this.ExcludeTags = excludeTags;
            this.Filters = filters;
        }

        public static RemoteRunOptions Default { get { return new RemoteRunOptions(new string[] { }, new string[] { }, new string[] { }); } }
    }
}