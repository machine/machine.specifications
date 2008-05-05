using System;
using System.Collections.Generic;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class StaticMigratorConfiguration : IConfiguration
  {
    #region Member Data
    private Type _connectionProviderType = typeof(SqlServerConnectionProvider);
    private Type _schemaProviderType = typeof(SqlServerSchemaProvider);
    private Type _databaseProviderType = typeof(SqlServerDatabaseProvider);
    private string _connectionString;
    private string _migrationsDirectory;
    private short _desiredVersion = -1;
    private bool _showDiagnostics;
    private string[] _references = new string[0];
    private int _commandTimeout = 30;
    #endregion

    #region StaticMigratorConfiguration()
    public StaticMigratorConfiguration()
    {
    }

    public StaticMigratorConfiguration(string connectionString, string migrationsDirectory)
    {
      _connectionString = connectionString;
      _migrationsDirectory = migrationsDirectory;
    }

    public StaticMigratorConfiguration(string connectionString, string migrationsDirectory, Type schemaProviderType)
    {
      _connectionString = connectionString;
      _migrationsDirectory = migrationsDirectory;
      _schemaProviderType = schemaProviderType;
    }

    public StaticMigratorConfiguration(string connectionString, string migrationsDirectory, Type schemaProviderType, Type connectionProviderType)
    {
      _connectionString = connectionString;
      _connectionProviderType = connectionProviderType;
      _schemaProviderType = schemaProviderType;
      _migrationsDirectory = migrationsDirectory;
    }
    #endregion

    #region IConfiguration Members
    public string[] References
    {
      get { return _references; }
      set { _references = value; }
    }

    public bool ShowDiagnostics
    {
      get { return _showDiagnostics; }
      set { _showDiagnostics = value; }
    }

    public short DesiredVersion
    {
      get { return _desiredVersion; }
      set { _desiredVersion = value; }
    }

    public string MigrationsDirectory
    {
      get { return _migrationsDirectory; }
      set { _migrationsDirectory = value; }
    }

    public string ConnectionString
    {
      get { return _connectionString; }
      set { _connectionString = value; }
    }

    public Type DatabaseProviderType
    {
      get { return _databaseProviderType; }
      set { _databaseProviderType = value; }
    }

    public Type SchemaProviderType
    {
      get { return _schemaProviderType; }
      set { _schemaProviderType = value; }
    }

    public Type ConnectionProviderType
    {
      get { return _connectionProviderType; }
      set { _connectionProviderType = value; }
    }

    public int CommandTimeout 
    {
      get { return _commandTimeout; }
      set { _commandTimeout = value; }
    }

    public void SetCommandTimeout(int commandTimeout)
    {
      this.CommandTimeout = commandTimeout;
    }

    public virtual Type TransactionProviderType
    {
      get { return typeof(TransactionProvider); }
    }
    #endregion
  }
}
