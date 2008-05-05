using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Container.Services.Impl
{
  [TestFixture]
  public class EmptyOverridesTests : ScaffoldTests<EmptyOverrides>
  {
    [Test]
    public void LookupOverride_Always_IsNull()
    {
      Assert.IsNull(_target.LookupOverride(null));
    }
  }
}