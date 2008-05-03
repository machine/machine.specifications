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
    public void GetHashCode_NullValue_Throws()
    {
      ValueTypeHelper.GetHashCode<Message1>(null);
    }
  }
}