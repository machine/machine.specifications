using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Core.Services;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigrationFinderTests : StandardFixture<MigrationFinder>
  {
    private IFileSystem _fileSystem;
    private IConfiguration _configuration;
    private List<string> _files;

    public override MigrationFinder Create()
    {
      _files = new List<string>();
      _files.AddRange(new String[] { "some_file.txt", "something.cs", "034veryclose.cs" });
      _fileSystem = _mocks.DynamicMock<IFileSystem>();
      _configuration = _mocks.DynamicMock<IConfiguration>();
      return new MigrationFinder(_fileSystem, _configuration);
    }

    [Test]
    public void FindMigrations_NoFiles_IsNoMigrations()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("MigrationsDirectory");
        SetupResult.For(_fileSystem.GetFiles("MigrationsDirectory")).Return(new string[0]);
      }
      CollectionAssert.IsEmpty(new List<Migration>(_target.FindMigrations()));
      _mocks.VerifyAll();
    }

    [Test]
    public void FindMigrations_NoFilesMatchingExpression_IsNoMigrations()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("MigrationsDirectory");
        SetupResult.For(_fileSystem.GetFiles("MigrationsDirectory")).Return(_files.ToArray());
      }
      CollectionAssert.IsEmpty(new List<Migration>(_target.FindMigrations()));
      _mocks.VerifyAll();
    }

    [Test]
    public void FindMigrations_HasMatchingFiles_IsThoseMigrations()
    {
      using (_mocks.Record())
      {
        _files.Add("001_migration.cs");
        SetupResult.For(_configuration.MigrationsDirectory).Return("MigrationsDirectory");
        SetupResult.For(_fileSystem.GetFiles("MigrationsDirectory")).Return(_files.ToArray());
      }
      List<Migration> migrations = new List<Migration>(_target.FindMigrations());
      Assert.AreEqual(1, migrations.Count);
      _mocks.VerifyAll();
    }
  }
}