using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueHelperTests_TypeHasOneField
  {
    [Test]
    public void AreEqual_WithDifferentValues_IsFalse()
    {
      Assert.IsFalse(ValueHelper.AreEqual(new Message2("B"), new Message2("A")));
    }

    [Test]
    public void AreEqual_WithEqualValues_IsTrue()
    {
      Assert.IsTrue(ValueHelper.AreEqual(new Message2("A"), new Message2("A")));
    }

    [Test]
    public void AreEqual_WithOneNull_IsFalse()
    {
      Assert.IsFalse(ValueHelper.AreEqual(new Message2(null), new Message2("A")));
    }

    [Test]
    public void AreEqual_WithBothNull_IsTrue()
    {
      Assert.IsTrue(ValueHelper.AreEqual(new Message2(null), new Message2(null)));
    }

    [Test]
    public void AreEqual_IsSameInstance_IsTrue()
    {
      Message2 value = new Message2("A");
      Assert.IsTrue(ValueHelper.AreEqual(value, value));
    }

    [Test]
    public void GetHashCode_WithEqualValues_AreEqual()
    {
      Assert.AreEqual(ValueHelper.GetHashCode(new Message2("A")), ValueHelper.GetHashCode(new Message2("A")));
    }

    [Test]
    public void GetHashCode_WithDifferentValues_AreNotEqual()
    {
      Assert.AreNotEqual(ValueHelper.GetHashCode(new Message2("B")), ValueHelper.GetHashCode(new Message2("A")));
    }

    [Test]
    public void GetHashCode_BothAreNull_AreEqual()
    {
      Assert.AreEqual(ValueHelper.GetHashCode(new Message2(null)), ValueHelper.GetHashCode(new Message2(null)));
    }

    [Test]
    public void GetHashCode_OneIsNull_AreNotEqual()
    {
      Assert.AreNotEqual(ValueHelper.GetHashCode(new Message2("A")), ValueHelper.GetHashCode(new Message2(null)));
    }
  }
}