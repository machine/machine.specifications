using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueHelperTests_TypeHasTwoFields
  {
    [Test]
    public void AreEqual_WithTwoSetsOfDifferentValues_IsFalse()
    {
      Assert.IsFalse(ValueHelper.AreEqual(new Message3("B", 1), new Message3("A", 2)));
    }

    [Test]
    public void AreEqual_WithOneSetOfDifferentValues_IsFalse()
    {
      Assert.IsFalse(ValueHelper.AreEqual(new Message3("A", 1), new Message3("A", 2)));
    }

    [Test]
    public void AreEqual_WithEqualValues_IsTrue()
    {
      Assert.IsTrue(ValueHelper.AreEqual(new Message3("A", 1), new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithEqualValues_AreEqual()
    {
      Assert.AreEqual(ValueHelper.GetHashCode(new Message3("A", 1)), ValueHelper.GetHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithTwoSetsOfDifferentValues_AreNotEqual()
    {
      Assert.AreNotEqual(ValueHelper.GetHashCode(new Message3("B", 2)), ValueHelper.GetHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithOneSetOfDifferentValues_AreNotEqual()
    {
      Assert.AreNotEqual(ValueHelper.GetHashCode(new Message3("A", 2)), ValueHelper.GetHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithOneSetOfNullValues_AreEqual()
    {
      Assert.AreEqual(ValueHelper.GetHashCode(new Message3(null, 1)), ValueHelper.GetHashCode(new Message3(null, 1)));
    }
  }
}