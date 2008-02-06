using System;
using System.Collections.Generic;
using System.Data;

using Machine.Migrations.Services;

using NHibernate;

namespace Machine.Migrations.NHibernate.Services.Impl
{
  public class NHibernateTransactionProvider : ITransactionProvider
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NHibernateTransactionProvider));
    #endregion

    #region Member Data
    private readonly INHibernateSessionProvider _sessionProvider;
    private ITransaction _transaction;
    #endregion

    #region NHibernateTransactionProvider()
    public NHibernateTransactionProvider(INHibernateSessionProvider sessionProvider)
    {
      _sessionProvider = sessionProvider;
    }
    #endregion

    #region IMigrationTransactionService Members
    public IDbTransaction Begin()
    {
      if (_transaction != null && !(_transaction.WasCommitted || _transaction.WasRolledBack))
      {
        throw new InvalidOperationException("Why is our previous transaction still open?");
      }
      ISession session = _sessionProvider.FindCurrentSession();
      _transaction = session.BeginTransaction();
      return new NHibernateTransactionWrapper(_transaction);
    }

    public void Enlist(IDbCommand command)
    {
      if (_transaction != null)
      {
        _transaction.Enlist(command);
      }
    }
    #endregion
  }
}
