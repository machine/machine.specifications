using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace ObjectFactories.Services.Impl
{
  [TestFixture]
  public class FactoryCallWeaverTests : ObjectFactoriesTests<FactoryCallWeaver>
  {
    public override FactoryCallWeaver Create()
    {
      return new FactoryCallWeaver(new NullLogger());
    }
  }
}
