using System;
using System.Collections.Generic;
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
    private readonly IConnectionProvider _connectionProvider;
    private readonly ITransactionProvider _transactionProvider;
    #endregion

    #region SqlServerDatabaseProvider()
    public SqlServerDatabaseProvider(IConnectionProvider connectionProvider, ITransactionProvider transactionProvider)
    {
      _connectionProvider = connectionProvider;
      _transactionProvider = transactionProvider;
    }
    #endregion

    #region IDatabaseProvider Members
    public void Open()
    {
      _connectionProvider.OpenConnection();
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

    public T[] ExecuteScalarArray<T>(string sql, params object[] objects)
    {
      IDataReader reader = ExecuteReader(sql, objects);
      List<T> values = new List<T>();
      while (reader.Read())
      {
        values.Add((T)reader.GetValue(0));
      }
      reader.Close();
      return values.ToArray();
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
      if (this.DatabaseConnection != null)
      {
        this.DatabaseConnection.Close();
      }
    }
    #endregion

    #region Member Data
    protected virtual IDbConnection DatabaseConnection
    {
      get { return _connectionProvider.CurrentConnection; }
    }

    private IDbCommand PrepareCommand(string sql, object[] objects)
    {
      IDbCommand command = this.DatabaseConnection.CreateCommand();
      command.CommandText = String.Format(sql, objects);
      _transactionProvider.Enlist(command);
      _log.Info(command.CommandText);
      return command;
    }
    #endregion
  }
}
