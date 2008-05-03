using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueTypeHelperTests_VeryComplexType
  {
    [Test]
    public void AreEqual_AllEqual_IsTrue()
    {
      DateTime when = DateTime.Now;
      TypeWithABunchOfTypes a = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      TypeWithABunchOfTypes b = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      Assert.IsTrue(ValueTypeHelper.AreEqual(a, b));
    }

    [Test]
    public void GetHashCode_AllEqual_AreEqual()
    {
      DateTime when = DateTime.Now;
      TypeWithABunchOfTypes a = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      TypeWithABunchOfTypes b = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      Assert.AreEqual(ValueTypeHelper.GetHashCode(a), ValueTypeHelper.GetHashCode(b));
    }
  }
}