using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.Example
{
  public class TestAssemblyContext : IAssemblyContext
  {
    public static bool OnAssemblyStartRun;
    public static bool OnAssemblyCompleteRun;

    public void OnAssemblyStart()
    {
      OnAssemblyStartRun = true;
    }

    public void OnAssemblyComplete()
    {
      OnAssemblyCompleteRun = true;
    }
  }
}
