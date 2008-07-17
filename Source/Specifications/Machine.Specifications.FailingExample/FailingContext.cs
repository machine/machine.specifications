using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications.FailingExample
{
  [Concern("Scott Bellware")]
  public class at_any_given_moment
  {
    It will_fail = ()=>
    {
      throw new Exception("hi scott, love you, miss you.");
    };
  }
}
