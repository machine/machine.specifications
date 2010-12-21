using System;
using System.Collections.Generic;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class RunOptions
  {
    public IEnumerable<string> IncludeTags { get; private set; }
    public IEnumerable<string> ExcludeTags { get; private set; }
    public IEnumerable<string> Filters { get; private set; }

    public RunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags, IEnumerable<string> filters)
    {
      IncludeTags = includeTags;
      ExcludeTags = excludeTags;
      Filters = filters;
    }

    public static RunOptions Default { get { return new RunOptions(new string[] {}, new string[] {}, new string[] {}); } }
  }
}
