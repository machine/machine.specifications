using System;
using System.Collections.Generic;

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
    private List<MigrationReference> _migrations;

    public override Migrator Create()
    {
      _migrations = new List<MigrationReference>();
      _databaseProvider = _mocks.DynamicMock<IDatabaseProvider>();
      _migrationSelector = _mocks.DynamicMock<IMigrationSelector>();
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrationRunner = _mocks.CreateMock<IMigrationRunner>();
      return new Migrator(_migrationSelector, _migrationRunner, _databaseProvider, _schemaStateManager);
    }

    [Test]
    public void RunMigrator_CanMigrate_RunsMigrations()
    {
      using (_mocks.Record())
      {
        _databaseProvider.Open();
        _schemaStateManager.CheckSchemaInfoTable();
        SetupResult.For(_migrationSelector.SelectMigrations()).Return(_migrations);
        SetupResult.For(_migrationRunner.CanMigrate(_migrations)).Return(true);
        _migrationRunner.Migrate(_migrations);
        _databaseProvider.Close();
      }
      _target.RunMigrator();
      _mocks.VerifyAll();
    }

    [Test]
    public void RunMigrator_CantMigrate_DoesNotRunMigrations()
    {
      using (_mocks.Record())
      {
        _databaseProvider.Open();
        _schemaStateManager.CheckSchemaInfoTable();
        SetupResult.For(_migrationSelector.SelectMigrations()).Return(_migrations);
        SetupResult.For(_migrationRunner.CanMigrate(_migrations)).Return(false);
        _databaseProvider.Close();
      }
      _target.RunMigrator();
      _mocks.VerifyAll();
    }
  }
}