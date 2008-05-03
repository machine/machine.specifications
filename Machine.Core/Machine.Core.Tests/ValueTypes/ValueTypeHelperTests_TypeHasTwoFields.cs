using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueTypeHelperTests_TypeHasTwoFields
  {
    [Test]
    public void AreEqual_WithTwoSetsOfDifferentValues_IsFalse()
    {
      Assert.IsFalse(ValueTypeHelper.AreEqual(new Message3("B", 1), new Message3("A", 2)));
    }

    [Test]
    public void AreEqual_WithOneSetOfDifferentValues_IsFalse()
    {
      Assert.IsFalse(ValueTypeHelper.AreEqual(new Message3("A", 1), new Message3("A", 2)));
    }

    [Test]
    public void AreEqual_WithEqualValues_IsTrue()
    {
      Assert.IsTrue(ValueTypeHelper.AreEqual(new Message3("A", 1), new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithEqualValues_AreEqual()
    {
      Assert.AreEqual(ValueTypeHelper.CalculateHashCode(new Message3("A", 1)), ValueTypeHelper.CalculateHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithTwoSetsOfDifferentValues_AreNotEqual()
    {
      Assert.AreNotEqual(ValueTypeHelper.CalculateHashCode(new Message3("B", 2)), ValueTypeHelper.CalculateHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithOneSetOfDifferentValues_AreNotEqual()
    {
      Assert.AreNotEqual(ValueTypeHelper.CalculateHashCode(new Message3("A", 2)), ValueTypeHelper.CalculateHashCode(new Message3("A", 1)));
    }

    [Test]
    public void GetHashCode_WithOneSetOfNullValues_AreEqual()
    {
      Assert.AreEqual(ValueTypeHelper.CalculateHashCode(new Message3(null, 1)), ValueTypeHelper.CalculateHashCode(new Message3(null, 1)));
    }

    [Test]
    public void ToString_AValue_IsString()
    {
      string value = ValueTypeHelper.ToString(new Message3("Jacob", 1));
      Console.WriteLine(value);
      Assert.IsTrue(value.Contains("Jacob"));
    }
  }
}