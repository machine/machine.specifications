using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public delegate void Context();
  public delegate void When();
  public delegate void Or_when();
  public delegate void It();
  public delegate void It_should_throw(Exception exception);
}