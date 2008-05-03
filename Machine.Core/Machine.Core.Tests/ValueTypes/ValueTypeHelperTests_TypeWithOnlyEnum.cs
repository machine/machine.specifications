using System;
using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueTypeHelperTests_TypeWithOnlyEnum
  {
    [Test]
    public void AreEqual_AllEqual_IsTrue()
    {
      TypeWithOnlyEnum a = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      TypeWithOnlyEnum b = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      Assert.IsTrue(ValueTypeHelper.AreEqual(a, b));
    }

    [Test]
    public void AreEqual_EnumIsDifferent_IsFalse()
    {
      TypeWithOnlyEnum a = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      TypeWithOnlyEnum b = new TypeWithOnlyEnum(YesNoMaybe.Maybe);
      Assert.IsFalse(ValueTypeHelper.AreEqual(a, b));
    }

    [Test]
    public void GetHashCode_AllEqual_AreEqual()
    {
      TypeWithOnlyEnum a = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      TypeWithOnlyEnum b = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      Assert.AreEqual(ValueTypeHelper.CalculateHashCode(a), ValueTypeHelper.CalculateHashCode(b));
    }

    [Test]
    public void GetHashCode_EnumIsDifferent_AreNotEqual()
    {
      TypeWithOnlyEnum a = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      TypeWithOnlyEnum b = new TypeWithOnlyEnum(YesNoMaybe.Maybe);
      Assert.AreNotEqual(ValueTypeHelper.CalculateHashCode(a), ValueTypeHelper.CalculateHashCode(b));
    }

    [Test]
    public void ToString_AValue_IsString()
    {
      TypeWithOnlyEnum a = new TypeWithOnlyEnum(YesNoMaybe.Yes);
      string value = ValueTypeHelper.ToString(a);
      Console.WriteLine(value);
      Assert.IsTrue(value.Contains("Yes"));
    }
  }
}