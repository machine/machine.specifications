using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Machine.Core.Services.Impl
{
  [TestFixture]
  public class NamerTests : StandardFixture<Namer>
  {
    public override Namer Create()
    {
      return new Namer();
    }

    [Test]
    public void ToCamelCase_IsCamelCase_DoesNothing()
    {
      Assert.AreEqual("ThisIsCamelCase", _target.ToCamelCase("ThisIsCamelCase"));
    }

    [Test]
    public void ToCamelCase_IsNotCamelCase_MakesCamelCase()
    {
      Assert.AreEqual("ThisIsCamelCase", _target.ToCamelCase("This_is_Camel_case"));
    }

    [Test]
    public void MakeRandomName_Always_MakesString()
    {
      Assert.AreEqual(8, _target.MakeRandomName().Length);
    }
  }
}