using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Machine.Migrations.Services.Impl
{
  public class SqlServerConnectionProvider : AbstractConnectionProvider
  {
    #region SqlServerConnectionProvider()
    public SqlServerConnectionProvider(IConfiguration configuration)
     : base(configuration)
    {
    }
    #endregion

    #region IConnectionProvider Members
    protected override IDbConnection CreateConnection(IConfiguration configuration)
    {
      return new SqlConnection(configuration.ConnectionString);
    }
    #endregion
  }
}
