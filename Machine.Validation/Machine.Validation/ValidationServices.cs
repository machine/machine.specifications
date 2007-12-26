using System;
using System.Collections.Generic;

using NSTM;

namespace Machine.Validation
{
  public class ValidationServices : IValidationServices
  {
    #region IValidationServices Members
    public T Wrap<T>(T validatable) where T : IValidatable
    {
      INstmTransaction tx = NstmMemory.BeginTransaction(NstmTransactionIsolationLevel.ReadCommitted);
      IValidationCallback callback = new ValidationCallback(tx);
      validatable.ValidationCallback = callback;
      return validatable;
    }
    #endregion
  }
  public class ValidationCallback : IValidationCallback
  {
    #region Member Data
    private readonly INstmTransaction _transaction;
    #endregion

    #region ValidationCallback()
    public ValidationCallback(INstmTransaction transaction)
    {
      _transaction = transaction;
    }
    #endregion

    #region IValidationCallback Members
    public void RollbackChanges()
    {
      _transaction.Rollback();
    }

    public void CommitChanges()
    {
      _transaction.Commit();
    }
    #endregion
  }
}
