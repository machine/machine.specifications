using System;
using System.Collections.Generic;
using System.Data;

using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace Machine.NHibernateExtensions.Listening
{
  public class ListeningNHibernateDriver : IDriver
  {
    #region Member Data
    private readonly IDriver _target;
    #endregion

    #region ListeningNHibernateDriver()
    public ListeningNHibernateDriver()
    {
      _target = new NHibernate.Driver.SqlClientDriver();
    }
    #endregion

    #region IDriver Members
    public void Configure(System.Collections.IDictionary settings)
    {
      _target.Configure(settings);
    }

    public IBatcher CreateBatcher(ConnectionManager connectionManager)
    {
      return _target.CreateBatcher(connectionManager);
    }

    public IDbConnection CreateConnection()
    {
      return new ListeningDbConnection(_target.CreateConnection());
    }

    public IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
    {
      return new ListeningDbCommand(_target.GenerateCommand(type, sqlString, parameterTypes));
    }

    public void PrepareCommand(IDbCommand command)
    {
      _target.PrepareCommand(command);
    }

    public bool SupportsMultipleOpenReaders
    {
      get { return _target.SupportsMultipleOpenReaders; }
    }

    public bool SupportsMultipleQueries
    {
      get { return _target.SupportsMultipleQueries; }
    }
    #endregion
  }
}