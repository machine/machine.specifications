using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public delegate void Establish();

  public delegate void Because();

  public delegate void It();
  public delegate object It_should_behave_like();

  public delegate void Cleanup();
}