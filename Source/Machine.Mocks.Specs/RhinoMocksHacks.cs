using System;
using Rhino.Mocks;
using Rhino.Mocks.Exceptions;

namespace Machine.Mocks.Specs
{
  public static class RhinoMocksHacks
  {
    public static void AssertWasNotCalled<T>(this T mock, Action<T> action)
    {
      try
      {
        mock.AssertWasCalled(action);
      }
      catch (ExpectationViolationException)
      {
        return;
      }

      throw new ExpectationViolationException("Expectd that something " + 
        " would not be called, but is was");
    }
  }
}