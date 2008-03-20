using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.Utility
{
  [TestFixture]
  public class ListHelperTests
  {
    [Test]
    public void Last_CollectionWithOneItem_IsLast()
    {
      Assert.AreEqual("Jacob", ListHelper.Last(new String[] { "Jacob" }));
    }

    [Test]
    public void Last_CollectionWithTwoItems_IsLast()
    {
      Assert.AreEqual("Andy", ListHelper.Last(new String[] { "Jacob", "Andy" }));
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Last_EmptyCollection_Throws()
    {
      ListHelper.Last(new String[0]);
    }
  }
}
