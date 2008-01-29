using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.Migrations.DatabaseProviders
{
  public interface IDatabaseProvider
  {
    IDbConnection DatabaseConnection
    {
      get;
    }
    void Open();
    object ExecuteScalar(string sql, params object[] objects);
    T ExecuteScalar<T>(string sql, params object[] objects);
    T[] ExecuteScalarArray<T>(string sql, params object[] objects);
    IDataReader ExecuteReader(string sql, params object[] objects);
    bool ExecuteNonQuery(string sql, params object[] objects);
    void Close();
    IDbTransaction Begin();
  }
}
