using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Machine.Migrations.Services.Impl
{
  public class ConnectionProvider : IConnectionProvider
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(ConnectionProvider));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    private IDbConnection _connection;
    #endregion

    #region ConnectionProvider()
    public ConnectionProvider(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    #endregion

    #region IConnectionProvider Members
    public IDbConnection OpenConnection()
    {
      return this.CurrentConnection;
    }

    public IDbConnection CurrentConnection
    {
      get
      {
        if (_connection == null)
        {
          _log.Info("Opening Connection: " + _configuration.ConnectionString);
          _connection = new SqlConnection(_configuration.ConnectionString);
          _connection.Open();
        }
        return _connection;
      }
    }
    #endregion
  }
}
