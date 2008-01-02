using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;
using Machine.Migrations.Services;

namespace Machine.Migrations
{
  public abstract class SimpleMigration : IDatabaseMigration
  {
    #region Member Data
    private readonly log4net.ILog _log;
    private ISchemaProvider _schemaProvider;
    private IDatabaseProvider _databaseProvider;
    private IConfiguration _configuration;
    #endregion

    #region Properties
    public log4net.ILog Log
    {
      get { return _log; }
    }

    public IConfiguration Configuration
    {
      get { return _configuration; }
    }

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
      _log = log4net.LogManager.GetLogger(GetType());
    }
    #endregion

    #region IDatabaseMigration Members
    public virtual void Initialize(IConfiguration configuration, IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _configuration = configuration;
      _schemaProvider = schemaProvider;
      _databaseProvider = databaseProvider;
    }

    public abstract void Up();

    public abstract void Down();
    #endregion
  }
}
