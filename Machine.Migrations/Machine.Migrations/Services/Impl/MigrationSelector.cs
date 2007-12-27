using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationSelector : IMigrationSelector
  {
    #region Member Data
    private readonly ISchemaStateManager _schemaStateManager;
    private readonly IMigrationFinder _migrationFinder;
    #endregion

    #region MigrationSelector()
    public MigrationSelector(ISchemaStateManager schemaStateManager, IMigrationFinder migrationFinder)
    {
      _schemaStateManager = schemaStateManager;
      _migrationFinder = migrationFinder;
    }
    #endregion

    #region IMigrationSelector Members
    public ICollection<Migration> SelectMigrations()
    {
      short version = _schemaStateManager.GetVersion();
      List<Migration> migrations = new List<Migration>();
      foreach (Migration migration in _migrationFinder.FindMigrations())
      {
        if (migration.Version > version)
        {
          migrations.Add(migration);
        }
      }
      return migrations;
    }
    #endregion
  }
}
