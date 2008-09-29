using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class RunOptions
  {
    public IEnumerable<string> IncludeTags { get; private set; }
    public IEnumerable<string> ExcludeTags { get; private set; }

    public RunOptions(IEnumerable<string> includeTags, IEnumerable<string> excludeTags)
    {
      IncludeTags = includeTags;
      ExcludeTags = excludeTags;
    }

    public static RunOptions Default { get { return new RunOptions(new string[] {}, new string[] {}); } }
  }
}
