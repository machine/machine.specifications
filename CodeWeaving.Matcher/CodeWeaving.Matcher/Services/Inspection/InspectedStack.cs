using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace CodeWeaving.Matcher.Services.Inspection
{
  public enum StackEntryType
  {
    Primitive,
    Reference,
    Variable
  }
  public class StackEntry
  {
  }
  public class InspectedStack
  {
    #region Member Data
    private readonly Stack<StackEntry> _stack = new Stack<StackEntry>();
    #endregion

    #region Methods
    public void Push(StackEntry value)
    {
      _stack.Push(value);
    }

    public void Pop()
    {
      _stack.Pop();
    }

    public void Clear()
    {
      _stack.Clear();
    }

    public int Count
    {
      get { return _stack.Count; }
    }
    #endregion
  }
}