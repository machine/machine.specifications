using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications.Model;

namespace Machine.Specifications.TDNetRunner
{
  public interface IResultFormatter
  {
    string FormatResult(Specification specification);
  }

  public class PassedResultFormatter : IResultFormatter
  {
    public string FormatResult(Specification specification)
    {
      return String.Format("  » {0}", specification.Name);
    }
  }

  public class FailedResultFormatter : IResultFormatter
  {
    public string FormatResult(Specification specification)
    {
      return String.Format("!!» {0} !!", specification.Name);
    }
  }

  public class UnknownResultFormatter : IResultFormatter
  {
    public string FormatResult(Specification specification)
    {
      return String.Format("??» {0} ??", specification.Name);
    }
  }
}
