using System;
using System.Collections.Generic;
using System.IO;

using ObjectFactories.CecilLayer;

using NUnit.Framework;

namespace ObjectFactories.Services.Impl
{
  [TestFixture]
  public class AssembliesTests : ObjectFactoriesTests<Assemblies>
  {
    public override Assemblies Create()
    {
      return new Assemblies();
    }

    [Test]
    public void LoadAssembly_GoodPath_GetsAssemblyFromCecil()
    {
      IAssembly assembly = _target.LoadAssembly(GetType().Assembly.Location);
      Assert.IsNotNull(assembly);
    }

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void LoadAssembly_BadPath_Throws()
    {
      _target.LoadAssembly(@"C:\MissingAssembly.dll");
    }

    [Test]
    public void SaveAssembly_Always_WritesNewAssembly()
    {
      string path = @"NewAssembly.dll";
      IAssembly assembly = _target.LoadAssembly(GetType().Assembly.Location);
      _target.SaveAssembly(assembly, path);
      Assert.IsTrue(File.Exists(path));
      File.Delete(path);
    }
  }
}
