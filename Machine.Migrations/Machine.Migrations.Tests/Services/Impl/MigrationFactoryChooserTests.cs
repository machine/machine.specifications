using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigrationFactoryChooserTests : StandardFixture<MigrationFactoryChooser>
  {
    private CSharpMigrationFactory _cSharpMigrationFactory;

    [Test]
    public void ChooseFactory_IsCSharp_ReturnsFactory()
    {
      Assert.AreEqual(_cSharpMigrationFactory, _target.ChooseFactory(new MigrationReference(1, "Migration", "001_migration.cs")));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ChooseFactory_IsNotCSharp_Throws()
    {
      _target.ChooseFactory(new MigrationReference(1, "Migration", "001_migration.boo"));
    }

    public override MigrationFactoryChooser Create()
    {
      _cSharpMigrationFactory = new CSharpMigrationFactory();
      return new MigrationFactoryChooser(_cSharpMigrationFactory);
    }
  }
}