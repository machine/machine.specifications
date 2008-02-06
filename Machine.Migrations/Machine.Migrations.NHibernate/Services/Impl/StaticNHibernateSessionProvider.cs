using System;
using System.Collections.Generic;

using NHibernate;

namespace Machine.Migrations.NHibernate.Services.Impl
{
  public class StaticNHibernateSessionProvider : INHibernateSessionProvider
  {
    #region Member Data
    private readonly ISession _session;
    #endregion

    #region StaticNHibernateSessionProvider()
    public StaticNHibernateSessionProvider(ISession session)
    {
      _session = session;
    }
    #endregion

    #region INHibernateSessionProvider Members
    public ISession FindCurrentSession()
    {
      return _session;
    }
    #endregion
  }
}
