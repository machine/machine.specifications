using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Runner
{
  [Serializable]
  public class RunOptions
  {
    public static RunOptions Default { get { return new RunOptions(); } }
  }
}
