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
    public bool CanMigrate(ICollection<MigrationReference> migrations)
    {
      return true;
    }

    public void Migrate(ICollection<MigrationReference> migrations)
    {
      foreach (MigrationReference migrationReference in migrations)
      {
        _log.InfoFormat("Running {0}", migrationReference.Path);
        IMigrationFactory migrationFactory = _migrationFactoryChooser.ChooseFactory(migrationReference);
        IDatabaseMigration migration = migrationFactory.CreateMigration(migrationReference);
        _migrationInitializer.InitializeMigration(migration);
        migration.Up();
        //_schemaStateManager.SetVersion(migration.Version);
      }
    }
    #endregion
  }
}
