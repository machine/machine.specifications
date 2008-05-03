using System;
using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueTypeHelperTests_TypeHasNoState
  {
    [Test]
    public void AreEqual_Always_IsFalse()
    {
      Assert.IsFalse(ValueTypeHelper.AreEqual(new Message1(), new Message1()));
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CalculateHashCode_NullValue_Throws()
    {
      ValueTypeHelper.CalculateHashCode<Message1>(null);
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ToString_NullValue_Throws()
    {
      ValueTypeHelper.ToString<Message1>(null);
    }

    [Test]
    public void ToString_AValue_IsString()
    {
      string value = ValueTypeHelper.ToString(new Message1());
      Console.WriteLine(value);
    }
  }
}