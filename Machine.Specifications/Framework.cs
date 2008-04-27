using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public delegate void Given();
  public delegate void When();
  public delegate void It();

  public interface IHasContext
  {
    void SetupContext();
  }

  public class Context : IHasContext
  {
    protected static Exception exception;

    public virtual void SetupContext()
    {
      
    }
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class SpecificationAttribute : Attribute
  {
    
  }
}