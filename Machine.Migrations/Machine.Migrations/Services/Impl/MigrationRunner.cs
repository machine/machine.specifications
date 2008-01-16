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
    private readonly IConfiguration _configuration;
    #endregion

    #region MigrationRunner()
    public MigrationRunner(IMigrationFactoryChooser migrationFactoryChooser, IMigrationInitializer migrationInitializer, ISchemaStateManager schemaStateManager, IConfiguration configuration)
    {
      _schemaStateManager = schemaStateManager;
      _configuration = configuration;
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
        using (Machine.Core.LoggingUtilities.Log4NetNdc.Push("{0}", step.MigrationReference.Name))
        {
          _log.InfoFormat("Running {0}", step);
          if (!_configuration.ShowDiagnostics)
          {
            step.Apply();
            if (step.Reverting)
            {
              _schemaStateManager.SetMigrationVersionUnapplied(step.Version);
            }
            else
            {
              _schemaStateManager.SetMigrationVersionApplied(step.Version);
            }
          }
        }
      }
    }
    #endregion
  }
}
