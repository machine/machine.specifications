using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public interface IAssemblyContext
  {
    void OnAssemblyStart();
    void OnAssemblyComplete();
  }
}
