using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Machine.Migrations.Services.Impl
{
  public class ConnectionProvider : IConnectionProvider
  {
    #region Member Data
    private readonly IConfiguration _configuration;
    #endregion

    #region ConnectionProvider()
    public ConnectionProvider(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    #endregion

    #region IConnectionProvider Members
    public IDbConnection CreateConnection()
    {
      IDbConnection connection = new SqlConnection(_configuration.ConnectionString);
      connection.Open();
      return connection;
    }
    #endregion
  }
}
