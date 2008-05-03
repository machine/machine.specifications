using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.ValueTypes
{
  [TestFixture]
  public class ValueHelperTests_VeryComplexType
  {
    [Test]
    public void AreEqual_AllEqual_IsTrue()
    {
      DateTime when = DateTime.Now;
      TypeWithABunchOfTypes a = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      TypeWithABunchOfTypes b = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      Assert.IsTrue(ValueHelper.AreEqual(a, b));
    }

    [Test]
    public void GetHashCode_AllEqual_AreEqual()
    {
      DateTime when = DateTime.Now;
      TypeWithABunchOfTypes a = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      TypeWithABunchOfTypes b = new TypeWithABunchOfTypes(true, 1, 2L, 3, "A", YesNoMaybe.Yes, when);
      Assert.AreEqual(ValueHelper.GetHashCode(a), ValueHelper.GetHashCode(b));
    }
  }
}