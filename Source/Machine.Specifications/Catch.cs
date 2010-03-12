using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machine.Specifications
{
  public static class Catch
  {
    public static Exception Exception(Action throwingAction)
    {
      try
      {
        throwingAction();
      }
      catch (Exception exception)
      {
        return exception;
      }

      return null;
    }
  }
}
