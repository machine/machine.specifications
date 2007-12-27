using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

using Machine.Migrations.Services;

namespace Machine.Migrations.DatabaseProviders
{
  public class SqlServerDatabaseProvider : IDatabaseProvider
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SqlServerDatabaseProvider));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    private SqlConnection _connection;
    #endregion

    #region SqlServerDatabaseProvider()
    public SqlServerDatabaseProvider(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    #endregion

    #region IDatabaseProvider Members
    public void Open()
    {
      _connection = new SqlConnection(_configuration.ConnectionString);
      _connection.Open();
    }

    public object ExecuteScalar(string sql, params object[] objects)
    {
      IDbCommand command = PrepareCommand(sql, objects);
      return command.ExecuteScalar();
    }

    public T ExecuteScalar<T>(string sql, params object[] objects)
    {
      IDbCommand command = PrepareCommand(sql, objects);
      return (T)command.ExecuteScalar();
    }

    public IDataReader ExecuteReader(string sql, params object[] objects)
    {
      IDbCommand command = PrepareCommand(sql, objects);
      return command.ExecuteReader();
    }

    public bool ExecuteNonQuery(string sql, params object[] objects)
    {
      IDbCommand command = PrepareCommand(sql, objects);
      command.ExecuteNonQuery();
      return true;
    }

    public void Close()
    {
      _connection.Close();
    }
    #endregion

    #region Member Data
    private IDbCommand PrepareCommand(string sql, object[] objects)
    {
      IDbCommand command = _connection.CreateCommand();
      command.CommandText = String.Format(sql, objects);
      _log.Info(command.CommandText);
      return command;
    }
    #endregion
  }
}
