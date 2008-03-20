using System;
using System.Collections.Generic;

namespace Machine.Container.Utility
{
  public class StackPopper<T> : IDisposable
  {
    #region Member Data
    private readonly Stack<T> _stack;
    #endregion

    #region StackPopper()
    protected StackPopper(Stack<T> stack, T value)
    {
      _stack = stack;
      _stack.Push(value);
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      _stack.Pop();
    }
    #endregion

    #region Methods
    public static IDisposable Push(Stack<T> stack, T value)
    {
      return new StackPopper<T>(stack, value);
    }
    #endregion
  }
}