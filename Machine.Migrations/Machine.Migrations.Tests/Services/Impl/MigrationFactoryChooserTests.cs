using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Core.Services;
using NUnit.Framework;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigrationFactoryChooserTests : StandardFixture<MigrationFactoryChooser>
  {
    private CSharpMigrationFactory _cSharpMigrationFactory;
    private BooMigrationFactory _booMigrationFactory;
    private IConfiguration _configuration;
    private IWorkingDirectoryManager _workingDirectoryManager;
    private IFileSystem _fileSystem;

    [Test]
    public void ChooseFactory_IsCSharp_ReturnsFactory()
    {
      Assert.AreEqual(_cSharpMigrationFactory, _target.ChooseFactory(new MigrationReference(1, "Migration", "001_migration.cs")));
    }

    [Test]
    public void ChooseFactory_IsBoo_ReturnsFactory()
    {
      Assert.AreEqual(_booMigrationFactory, _target.ChooseFactory(new MigrationReference(1, "Migration", "001_migration.boo")));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ChooseFactory_IsNotCSharpOrBoo_Throws()
    {
      _target.ChooseFactory(new MigrationReference(1, "Migration", "001_migration.vb"));
    }

    public override MigrationFactoryChooser Create()
    {
      _configuration = _mocks.DynamicMock<IConfiguration>();
      _fileSystem = _mocks.DynamicMock<IFileSystem>();
      _workingDirectoryManager = _mocks.DynamicMock<IWorkingDirectoryManager>();
      _cSharpMigrationFactory = new CSharpMigrationFactory(_configuration, _fileSystem, _workingDirectoryManager);
      _booMigrationFactory = new BooMigrationFactory(_configuration, _workingDirectoryManager);
      return new MigrationFactoryChooser(_cSharpMigrationFactory, _booMigrationFactory);
    }
  }
}