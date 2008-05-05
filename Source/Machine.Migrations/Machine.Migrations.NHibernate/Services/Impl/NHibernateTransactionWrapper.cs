using System;
using System.Data;
using NHibernate;

namespace Machine.Migrations.NHibernate.Services.Impl
{
  public class NHibernateTransactionWrapper : IDbTransaction
  {
    #region Member Data
    private readonly ITransaction _transaction;
    #endregion

    #region NHibernateTransactionWrapper()
    public NHibernateTransactionWrapper(ITransaction transaction)
    {
      _transaction = transaction;
    }
    #endregion

    #region IDbTransaction Members
    public void Commit()
    {
      _transaction.Commit();
    }

    public IDbConnection Connection
    {
      get { throw new NotImplementedException(); }
    }

    public IsolationLevel IsolationLevel
    {
      get { throw new NotImplementedException(); }
    }

    public void Rollback()
    {
      _transaction.Rollback();
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      _transaction.Dispose();
    }
    #endregion
  }
}