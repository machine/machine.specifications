using System;
using System.Collections.Generic;
using System.Data;

using Machine.Migrations.Services;

namespace Machine.Migrations.NHibernate.Services.Impl
{
  public class NHibernateConnectionProvider : IConnectionProvider
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NHibernateConnectionProvider));
    #endregion

    #region Member Data
    private readonly INHibernateSessionProvider _sessionProvider;
    #endregion

    #region NHibernateConnectionProvider()
    public NHibernateConnectionProvider(INHibernateSessionProvider sessionProvider)
    {
      _sessionProvider = sessionProvider;
    }
    #endregion

    #region IConnectionProvider Members
    public IDbConnection OpenConnection()
    {
      return this.CurrentConnection;
    }

    public IDbConnection CurrentConnection
    {
      get { return _sessionProvider.FindCurrentSession().Connection; }
    }
    #endregion
  }
}
