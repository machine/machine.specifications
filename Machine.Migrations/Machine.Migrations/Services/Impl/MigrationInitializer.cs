using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationInitializer : IMigrationInitializer
  {
    #region Member Data
    private readonly IConfiguration _configuration;
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ISchemaProvider _schemaProvider;
    #endregion

    #region MigrationInitializer()
    public MigrationInitializer(IConfiguration configuration, IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _configuration = configuration;
      _databaseProvider = databaseProvider;
      _schemaProvider = schemaProvider;
    }
    #endregion

    #region IMigrationInitializer Members
    public void InitializeMigration(IDatabaseMigration migration)
    {
      migration.Initialize(_configuration, _databaseProvider, _schemaProvider);
    }
    #endregion
  }
}
