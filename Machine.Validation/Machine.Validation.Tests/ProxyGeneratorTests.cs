using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Machine.Validation
{
  [TestFixture]
  public class ProxyGeneratorTests
  {
    [Test]
    public void Generate()
    {
      ProxyGenerator generator = new ProxyGenerator();
      Type generated = generator.GenerateProxy(typeof(DumbUser));
    }
  }
}
