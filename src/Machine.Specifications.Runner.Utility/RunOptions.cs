using System;
using System.Collections.Generic;
using System.Linq;

namespace Machine.Specifications.Runner.Utility
{
    [Serializable]
    public class RunOptions
    {
        private RunOptions()
        {
            IncludeTags = Enumerable.Empty<string>();
            ExcludeTags = Enumerable.Empty<string>();
            Filters = Enumerable.Empty<string>();
        }

        public IEnumerable<string> IncludeTags { get; protected set; }

        public IEnumerable<string> ExcludeTags { get; protected set; }

        public IEnumerable<string> Filters { get; protected set; }

        public string ShadowCopyCachePath { get; protected set; }

        public static RunOptions Default => new RunOptions();

        public static RunOptionsBuilder Custom => new RunOptionsBuilder();

        public class RunOptionsBuilder : RunOptions 
        {
            internal RunOptionsBuilder()
            {
            }

            public RunOptionsBuilder Include(IEnumerable<string> tags)
            {
                IncludeTags = tags;

                return this;
            }

            public RunOptionsBuilder Exclude(IEnumerable<string> tags)
            {
                ExcludeTags = tags;

                return this;
            }

            public RunOptionsBuilder FilterBy(IEnumerable<string> filters)
            {
                Filters = filters;

                return this;
            }

            public RunOptionsBuilder ShadowCopyTo(string cachePath)
            {
                ShadowCopyCachePath = cachePath;
                return this;
            }
        }
    }
}
