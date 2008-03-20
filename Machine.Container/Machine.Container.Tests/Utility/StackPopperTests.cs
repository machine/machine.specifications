using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Container.Utility
{
  [TestFixture]
  public class StackPopperTests : BunkerTests
  {
    #region Member Data
    private Stack<string> _stack;
    #endregion

    #region Test Setup and Teardown Methods
    public override void Setup()
    {
      base.Setup();
      _stack = new Stack<string>();
    }
    #endregion

    #region Test Methods
    [Test]
    public void Push_Always_DoesThat()
    {
      CollectionAssert.IsEmpty(_stack);
      using (StackPopper<string>.Push(_stack, "Jacob"))
      {
        Assert.AreEqual("Jacob", _stack.Peek());
      }
      CollectionAssert.IsEmpty(_stack);
    }
    #endregion
  }
}
