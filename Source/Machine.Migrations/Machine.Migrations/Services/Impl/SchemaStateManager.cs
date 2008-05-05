using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class SchemaStateManager : ISchemaStateManager
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SchemaStateManager));
    #endregion

    #region Member Data
    private readonly string TableName = "schema_info";
    private readonly string IdColumnName = "id";
    // private readonly string ApplicationDateColumnName = "applied_at";
    private readonly string VersionColumnName = "version";
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ISchemaProvider _schemaProvider;
    #endregion

    #region SchemaStateManager()
    public SchemaStateManager(IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _databaseProvider = databaseProvider;
      _schemaProvider = schemaProvider;
    }
    #endregion

    #region ISchemaStateManager Members
    public void CheckSchemaInfoTable()
    {
      if (_schemaProvider.HasTable(TableName))
      {
        return;
      }

      _log.InfoFormat("Creating {0}...", TableName);

      Column[] columns = new Column[] {
        new Column(IdColumnName, typeof(Int32), 4, true),
        new Column(VersionColumnName, typeof(Int32), 4, false)
        // new Column(ApplicationDateColumnName, typeof(DateTime), 0, false)
      };
      _schemaProvider.AddTable(TableName, columns);
    }

    public short[] GetAppliedMigrationVersions()
    {
      return _databaseProvider.ExecuteScalarArray<Int16>("SELECT CAST({1} AS SMALLINT) FROM {0} ORDER BY {1}", TableName, VersionColumnName);
    }

    public void SetMigrationVersionUnapplied(short version)
    {
      _databaseProvider.ExecuteNonQuery("DELETE FROM {0} WHERE {1} = {2}", TableName, VersionColumnName, version);
    }

    public void SetMigrationVersionApplied(short version)
    {
      _databaseProvider.ExecuteNonQuery("INSERT INTO {0} ({2}) VALUES ({4})", TableName, IdColumnName, VersionColumnName, version, version);
    }
    #endregion
  }
}
