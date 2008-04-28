using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public delegate void Before();
  public delegate void When();
  public delegate void It();
  public delegate void It_should_throw(Exception exception);
  public delegate void After();

  [AttributeUsage(AttributeTargets.Class)]
  public class SpecificationAttribute : Attribute
  {
    
  }
}