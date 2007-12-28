using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigrationRunnerTests : StandardFixture<MigrationRunner>
  {
    private ISchemaStateManager _schemaStateManager;
    private IMigrationFactoryChooser _migrationFactoryChooser;
    private IMigrationInitializer _migrationInitializer;
    private IMigrationFactory _migrationFactory;
    private IConfiguration _configuration;
    private IDatabaseMigration _migration1;
    private IDatabaseMigration _migration2;
    private List<MigrationStep> _steps;

    public override MigrationRunner Create()
    {
      _steps = new List<MigrationStep>();
      _steps.Add(new MigrationStep(new MigrationReference(1, "A", "001_a.cs"), false));
      _steps.Add(new MigrationStep(new MigrationReference(2, "B", "002_b.cs"), false));
      _migration1 = _mocks.CreateMock<IDatabaseMigration>();
      _migration2 = _mocks.CreateMock<IDatabaseMigration>();
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrationFactoryChooser = _mocks.DynamicMock<IMigrationFactoryChooser>();
      _migrationInitializer = _mocks.DynamicMock<IMigrationInitializer>();
      _migrationFactory = _mocks.DynamicMock<IMigrationFactory>();
      _configuration = _mocks.DynamicMock<IConfiguration>();
      return new MigrationRunner(_migrationFactoryChooser, _migrationInitializer, _schemaStateManager, _configuration);
    }

    [Test]
    public void CanMigrate_Always_GetsFactoryAndInitializes()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_migrationFactoryChooser.ChooseFactory(_steps[0].MigrationReference)).Return(_migrationFactory);
        SetupResult.For(_migrationFactoryChooser.ChooseFactory(_steps[1].MigrationReference)).Return(_migrationFactory);
        SetupResult.For(_migrationFactory.CreateMigration(_steps[0].MigrationReference)).Return(_migration1);
        SetupResult.For(_migrationFactory.CreateMigration(_steps[1].MigrationReference)).Return(_migration2);
        _migrationInitializer.InitializeMigration(_migration1);
        _migrationInitializer.InitializeMigration(_migration2);
      }
      _target.CanMigrate(_steps);
      _mocks.VerifyAll();
      Assert.AreEqual(_migration1, _steps[0].DatabaseMigration);
      Assert.AreEqual(_migration2, _steps[1].DatabaseMigration);
    }

    [Test]
    public void Migrate_NoDiagnostics_Applies()
    {
      _steps[0].DatabaseMigration = _migration1;
      _steps[1].DatabaseMigration = _migration2;
      using (_mocks.Record())
      {
        _migration1.Up();
        _schemaStateManager.SetVersion(1);
        _migration2.Up();
        _schemaStateManager.SetVersion(2);
      }
      _target.Migrate(_steps);
      _mocks.VerifyAll();
    }

    [Test]
    public void Migrate_Diagnostics_DoesNotApply()
    {
      _steps[0].DatabaseMigration = _migration1;
      _steps[1].DatabaseMigration = _migration2;
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.ShowDiagnostics).Return(true);
      }
      _target.Migrate(_steps);
      _mocks.VerifyAll();
    }
  }
}