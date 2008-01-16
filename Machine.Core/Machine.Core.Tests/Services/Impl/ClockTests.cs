using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.Services.Impl
{
  [TestFixture]
  public class ClockTests : StandardFixture<Clock>
  {
    [Test]
    public void CurrentTime_Always_IsTime()
    {
      DateTime now = _target.CurrentTime;
      Assert.Less(DateTime.Now - now, TimeSpan.FromMilliseconds(1000));
    }

    public override Clock Create()
    {
      return new Clock();
    }
  }
}