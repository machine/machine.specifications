using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.Migrations.Services.Impl
{
  public class TransactionProvider : ITransactionProvider
  {
    #region Member Data
    private readonly IConnectionProvider _connectionProvider;
    private IDbTransaction _transaction;
    #endregion

    #region MigrationTransactionService()
    public TransactionProvider(IConnectionProvider connectionProvider)
    {
      _connectionProvider = connectionProvider;
    }
    #endregion

    #region IMigrationTransactionService Members
    public IDbTransaction Begin()
    {
      return _transaction = _connectionProvider.CurrentConnection.BeginTransaction();
    }

    public void Enlist(IDbCommand command)
    {
      command.Transaction = _transaction;
    }
    #endregion
  }
}
