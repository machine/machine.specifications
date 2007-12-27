using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationRunner : IMigrationRunner
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MigrationRunner));
    #endregion

    #region Member Data
    private readonly IMigrationFactoryChooser _migrationFactoryChooser;
    private readonly IMigrationInitializer _migrationInitializer;
    private readonly ISchemaStateManager _schemaStateManager;
    #endregion

    #region MigrationRunner()
    public MigrationRunner(IMigrationFactoryChooser migrationFactoryChooser, IMigrationInitializer migrationInitializer, ISchemaStateManager schemaStateManager)
    {
      _schemaStateManager = schemaStateManager;
      _migrationInitializer = migrationInitializer;
      _migrationFactoryChooser = migrationFactoryChooser;
    }
    #endregion

    #region IMigrationRunner Members
    public bool CanMigrate(ICollection<MigrationStep> steps)
    {
      foreach (MigrationStep step in steps)
      {
        MigrationReference migrationReference = step.MigrationReference;
        IMigrationFactory migrationFactory = _migrationFactoryChooser.ChooseFactory(migrationReference);
        IDatabaseMigration migration = migrationFactory.CreateMigration(migrationReference);
        step.DatabaseMigration = migration;
        _migrationInitializer.InitializeMigration(migration);
      }
      _log.Info("All migrations are initialized.");
      return true;
    }

    public void Migrate(ICollection<MigrationStep> steps)
    {
      foreach (MigrationStep step in steps)
      {
        _log.InfoFormat("Running {0}", step);
        step.Apply();
        _schemaStateManager.SetVersion(step.VersionAfterApplying);
      }
    }
    #endregion
  }
}
