using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  [DelegateUsage(DelegateUsage.Setup)]
  public delegate void Establish();

  [DelegateUsage(DelegateUsage.Act)]
  public delegate void Because();

  [DelegateUsage(DelegateUsage.Assert)]
  public delegate void It();
  [DelegateUsage(DelegateUsage.Behavior)]
  public delegate void Behaves_like<TBehavior>();

  [DelegateUsage(DelegateUsage.Cleanup)]
  public delegate void Cleanup();
}