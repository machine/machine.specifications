using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations
{
  public abstract class SimpleMigration : IDatabaseMigration
  {
    #region Member Data
    private ISchemaProvider _schemaProvider;
    private IDatabaseProvider _databaseProvider;
    #endregion

    #region Properties
    public ISchemaProvider Schema
    {
      get { return _schemaProvider; }
    }

    public IDatabaseProvider Database
    {
      get { return _databaseProvider; }
    }
    #endregion

    #region SimpleMigration()
    protected SimpleMigration()
    {
    }
    #endregion

    #region IDatabaseMigration Members
    public virtual void Initialize(IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _schemaProvider = schemaProvider;
      _databaseProvider = databaseProvider;
    }

    public abstract void Up();

    public abstract void Down();
    #endregion
  }
}
