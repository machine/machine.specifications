using System;
using System.Collections.Generic;

using Machine.Container.Services;

using NUnit.Framework;

namespace Machine.Container
{
  [TestFixture]
  public class IoCTests : BunkerTests
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Container_WhenItsNull_Throws()
    {
      IoC.Container = null;
      IHighLevelContainer container = IoC.Container;
    }

    [Test]
    public void Container_GetAndSet_Works()
    {
      IHighLevelContainer container = _mocks.DynamicMock<IHighLevelContainer>();
      IoC.Container = container;
      Assert.AreEqual(container, IoC.Container);
    }
  }
}
