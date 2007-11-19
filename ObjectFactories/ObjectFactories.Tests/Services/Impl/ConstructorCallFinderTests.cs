using System;
using System.Collections.Generic;

using Mono.Cecil;

using ObjectFactories.CecilLayer;
using ObjectFactories.Model;

using NUnit.Framework;

namespace ObjectFactories.Services.Impl
{
  [TestFixture]
  public class ConstructorCallFinderTests : ObjectFactoriesTests<ConstructorCallFinder>
  {
    public override ConstructorCallFinder Create()
    {
      return new ConstructorCallFinder(new NullLogger());
    }
  }
}
