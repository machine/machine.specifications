using System;
using System.Collections.Generic;
using System.Data;
using Machine.Migrations.DatabaseProviders;

namespace Machine.Migrations.Services.Impl
{
  public class Migrator : IMigrator
  {
    #region Member Data
    private readonly IDatabaseProvider _databaseProvider;
    private readonly IMigrationSelector _migrationSelector;
    private readonly IMigrationRunner _migrationRunner;
    private readonly ISchemaStateManager _schemaStateManager;
    private readonly IWorkingDirectoryManager _workingDirectoryManager;
    #endregion

    #region Migrator()
    public Migrator(IMigrationSelector migrationSelector, IMigrationRunner migrationRunner, IDatabaseProvider databaseProvider, ISchemaStateManager schemaStateManager, IWorkingDirectoryManager workingDirectoryManager)
    {
      _migrationSelector = migrationSelector;
      _workingDirectoryManager = workingDirectoryManager;
      _schemaStateManager = schemaStateManager;
      _databaseProvider = databaseProvider;
      _migrationRunner = migrationRunner;
    }
    #endregion

    #region IMigrator Members
    public void RunMigrator()
    {
      IDbTransaction transaction = null;
      try
      {
        _workingDirectoryManager.Create();
        _databaseProvider.Open();
        transaction = _databaseProvider.Begin();
        _schemaStateManager.CheckSchemaInfoTable();
        ICollection<MigrationStep> steps = _migrationSelector.SelectMigrations();
        if (_migrationRunner.CanMigrate(steps))
        {
          _migrationRunner.Migrate(steps);
        }
        transaction.Commit();
      }
      catch (Exception)
      {
        if (transaction != null)
        {
          transaction.Rollback();
        }
        throw;
      }
      finally
      {
        _databaseProvider.Close();
      }
    }
    #endregion
  }
}
