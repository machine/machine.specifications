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
    private readonly ISchemaStateManager _schemaStateManager;
    private readonly IMigrationFactoryChooser _cigrationFactoryChooser;
    #endregion

    #region MigrationRunner()
    public MigrationRunner(ISchemaStateManager schemaStateManager, IMigrationFactoryChooser migrationFactoryChooser)
    {
      _schemaStateManager = schemaStateManager;
      _cigrationFactoryChooser = migrationFactoryChooser;
    }
    #endregion

    #region IMigrationRunner Members
    public bool CanMigrate(ICollection<MigrationReference> migrations)
    {
      return true;
    }

    public void Migrate(ICollection<MigrationReference> migrations)
    {
      foreach (MigrationReference migration in migrations)
      {
        _log.InfoFormat("Running {0}", migration.Path);
        IMigrationFactory migrationFactory = _cigrationFactoryChooser.ChooseFactory(migration);
        IDatabaseMigration instance = migrationFactory.CreateMigration(migration);
        instance.Up();
        //_schemaStateManager.SetVersion(migration.Version);
      }
    }
    #endregion
  }
}
