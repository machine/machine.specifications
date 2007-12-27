using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationInitializer : IMigrationInitializer
  {
    #region Member Data
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ISchemaProvider _schemaProvider;
    #endregion

    #region MigrationInitializer()
    public MigrationInitializer(IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _databaseProvider = databaseProvider;
      _schemaProvider = schemaProvider;
    }
    #endregion

    #region IMigrationInitializer Members
    public void InitializeMigration(IDatabaseMigration migration)
    {
      migration.Initialize(_databaseProvider, _schemaProvider);
    }
    #endregion
  }
}
