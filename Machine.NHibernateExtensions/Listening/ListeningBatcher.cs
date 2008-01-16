using System;
using System.Data;

using NHibernate.AdoNet;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace Machine.NHibernateExtensions.Listening
{
  public class ListeningBatcher : IBatcher
  {
    #region IBatcher Members
    public void AbortBatch(Exception e)
    {
      throw new NotImplementedException();
    }

    public void AddToBatch(IExpectation expectation)
    {
      throw new NotImplementedException();
    }

    public int BatchSize
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public void CancelLastQuery()
    {
      throw new NotImplementedException();
    }

    public void CloseCommand(IDbCommand cmd, IDataReader reader)
    {
      throw new NotImplementedException();
    }

    public void CloseCommands()
    {
      throw new NotImplementedException();
    }

    public void ExecuteBatch()
    {
      throw new NotImplementedException();
    }

    public int ExecuteNonQuery(IDbCommand cmd)
    {
      throw new NotImplementedException();
    }

    public IDataReader ExecuteReader(IDbCommand cmd)
    {
      throw new NotImplementedException();
    }

    public bool HasOpenResources
    {
      get { throw new NotImplementedException(); }
    }

    public IDbCommand PrepareBatchCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes)
    {
      throw new NotImplementedException();
    }

    public IDbCommand PrepareCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes)
    {
      throw new NotImplementedException();
    }

    public IDbCommand PrepareQueryCommand(CommandType commandType, SqlString sql, SqlType[] parameterTypes)
    {
      throw new NotImplementedException();
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}