using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.Utility
{
  [TestFixture]
  public class TimeSpanHelperTests
  {
    [Test]
    public void ToPrettyString_OneDay_IsOneDay()
    {
      string actual = TimeSpanHelper.ToPrettyString(TimeSpan.FromDays(1.0));
      Assert.AreEqual("1 day", actual);
    }

    [Test]
    public void ToPrettyString_FewSeconds_IsSeconds()
    {
      string actual = TimeSpanHelper.ToPrettyString(TimeSpan.FromSeconds(3.0));
      Assert.AreEqual("3 seconds", actual);
    }

    [Test]
    public void ToPrettyString_SeveralDays_IsFewDays()
    {
      string actual = TimeSpanHelper.ToPrettyString(TimeSpan.FromDays(15.0));
      Assert.AreEqual("2 weeks", actual);
    }
  }
}
