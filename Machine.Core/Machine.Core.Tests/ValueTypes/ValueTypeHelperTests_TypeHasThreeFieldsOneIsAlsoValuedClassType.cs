using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueTypeHelperTests_TypeHasThreeFieldsOneIsAlsoValuedClassType
  {
    [Test]
    public void AreEqual_AllEqualButClassMemberIsReferenceEqual_IsTrue()
    {
      Message2 m3 = new Message2("C");
      Assert.IsTrue(ValueTypeHelper.AreEqual(new Message4("A", 1, m3), new Message4("A", 1, m3)));
    }

    [Test]
    public void AreEqual_AllEqual_IsTrue()
    {
      Assert.IsTrue(ValueTypeHelper.AreEqual(new Message4("A", 1, new Message2("C")), new Message4("A", 1, new Message2("C"))));
    }

    [Test]
    public void AreEqual_ClassMemberIsNotEqual_IsFalse()
    {
      Assert.IsFalse(ValueTypeHelper.AreEqual(new Message4("A", 1, new Message2("D")), new Message4("A", 1, new Message2("C"))));
    }

    [Test]
    public void GetHashCode_ClassMemberIsNotEqual_AreNotEqual()
    {
      Message4 aMessage4 = new Message4("A", 1, new Message2("B"));
      Message4 bMessage4 = new Message4("A", 1, new Message2("C"));
      Assert.AreNotEqual(ValueTypeHelper.CalculateHashCode(aMessage4), ValueTypeHelper.CalculateHashCode(bMessage4));
    }
  }
}