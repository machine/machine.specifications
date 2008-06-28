using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.FailingExample
{
  [Concerning("Epic Failure")]
  public class FailingContext
  {
    It should_fail = ()=>
    {
      throw new Exception("hi");
    };
  }
}
