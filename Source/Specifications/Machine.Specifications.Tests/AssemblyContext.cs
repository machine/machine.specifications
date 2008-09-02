using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public class AssemblyContext : IAssemblyContext
  {
    public void OnAssemblyStart()
    {
      throw new NotFiniteNumberException("fail");
    }

    public void OnAssemblyComplete()
    {
    }
  }
}
