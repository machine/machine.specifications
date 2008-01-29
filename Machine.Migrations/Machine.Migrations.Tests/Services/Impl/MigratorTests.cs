using System;
using System.Collections.Generic;
using System.Data;
using Machine.Core;
using Machine.Migrations.DatabaseProviders;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigratorTests : StandardFixture<Migrator>
  {
    private IDatabaseProvider _databaseProvider;
    private IMigrationSelector _migrationSelector;
    private IMigrationRunner _migrationRunner;
    private ISchemaStateManager _schemaStateManager;
    private IWorkingDirectoryManager _workingDirectoryManager;
    private IDbTransaction _transaction;
    private List<MigrationStep> _steps;

    public override Migrator Create()
    {
      _steps = new List<MigrationStep>();
      _databaseProvider = _mocks.DynamicMock<IDatabaseProvider>();
      _migrationSelector = _mocks.DynamicMock<IMigrationSelector>();
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrationRunner = _mocks.CreateMock<IMigrationRunner>();
      _workingDirectoryManager = _mocks.CreateMock<IWorkingDirectoryManager>();
      _transaction = _mocks.CreateMock<IDbTransaction>();
      return new Migrator(_migrationSelector, _migrationRunner, _databaseProvider, _schemaStateManager, _workingDirectoryManager);
    }

    [Test]
    public void RunMigrator_CanMigrate_RunsMigrations()
    {
      using (_mocks.Record())
      {
        _databaseProvider.Open();
        Expect.Call(_databaseProvider.Begin()).Return(_transaction);
        _schemaStateManager.CheckSchemaInfoTable();
        SetupResult.For(_migrationSelector.SelectMigrations()).Return(_steps);
        _workingDirectoryManager.Create();
        SetupResult.For(_migrationRunner.CanMigrate(_steps)).Return(true);
        _migrationRunner.Migrate(_steps);
        _transaction.Commit();
        _databaseProvider.Close();
      }
      _target.RunMigrator();
      _mocks.VerifyAll();
    }

    [Test]
    public void RunMigrator_Errors_RollsBack()
    {
      using (_mocks.Record())
      {
        _databaseProvider.Open();
        Expect.Call(_databaseProvider.Begin()).Return(_transaction);
        _schemaStateManager.CheckSchemaInfoTable();
        SetupResult.For(_migrationSelector.SelectMigrations()).Return(_steps);
        _workingDirectoryManager.Create();
        SetupResult.For(_migrationRunner.CanMigrate(_steps)).Return(true);
        _migrationRunner.Migrate(_steps);
        LastCall.Throw(new ArgumentException());
        _transaction.Rollback();
        _databaseProvider.Close();
      }
      bool caught = false;
      try
      {
        _target.RunMigrator();
      }
      catch (ArgumentException)
      {
        caught = true;
      }
      Assert.IsTrue(caught);
      _mocks.Verify(_transaction);
    }

    [Test]
    public void RunMigrator_CantMigrate_DoesNotRunMigrations()
    {
      using (_mocks.Record())
      {
        _databaseProvider.Open();
        Expect.Call(_databaseProvider.Begin()).Return(_transaction);
        _schemaStateManager.CheckSchemaInfoTable();
        SetupResult.For(_migrationSelector.SelectMigrations()).Return(_steps);
        _workingDirectoryManager.Create();
        SetupResult.For(_migrationRunner.CanMigrate(_steps)).Return(false);
        _transaction.Commit();
        _databaseProvider.Close();
      }
      _target.RunMigrator();
      _mocks.VerifyAll();
    }
  }
}