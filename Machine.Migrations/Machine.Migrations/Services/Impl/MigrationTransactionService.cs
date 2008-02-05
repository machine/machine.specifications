using System;
using System.Collections.Generic;
using System.Data;
using Machine.Migrations.DatabaseProviders;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationTransactionService : IMigrationTransactionService
  {
    #region Member Data
    private readonly IDatabaseProvider _databaseProvider;
    #endregion

    #region MigrationTransactionService()
    public MigrationTransactionService(IDatabaseProvider databaseProvider)
    {
      _databaseProvider = databaseProvider;
    }
    #endregion

    #region IMigrationTransactionService Members
    public IDbTransaction Begin()
    {
      return _databaseProvider.Begin();
    }
    #endregion
  }
}
