using System;
using System.Collections.Generic;
using System.Data;

using NHibernate.Connection;

namespace Machine.NHibernateExtensions.Listening
{
  public class ListeningDriverConnectionProvider : ConnectionProvider
  {
    #region IConnectionProvider Members
    public override void CloseConnection(IDbConnection connection)
    {
      base.CloseConnection(connection);
      connection.Dispose();
    }

    public override IDbConnection GetConnection()
    {
      IDbConnection connection = new ListeningDbConnection(base.Driver.CreateConnection());
      connection.ConnectionString = this.ConnectionString;
      connection.Open();
      return connection;
    }
    #endregion
  }
}
