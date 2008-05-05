using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.NHibernateExtensions.Listening
{
  public class ListeningDbCommand : IDbCommand
  {
    #region Member Data
    private readonly IDbCommand _target;
    #endregion

    #region ListeningDbCommand()
    public ListeningDbCommand(IDbCommand target)
    {
      _target = target;
    }
    #endregion

    #region IDbCommand Members
    public void Cancel()
    {
      _target.Cancel();
    }

    public string CommandText
    {
      get { return _target.CommandText; }
      set { _target.CommandText = value; }
    }

    public int CommandTimeout
    {
      get { return _target.CommandTimeout; }
      set { _target.CommandTimeout = value; }
    }

    public CommandType CommandType
    {
      get { return _target.CommandType; }
      set { _target.CommandType = value; }
    }

    public IDbConnection Connection
    {
      get
      {
        return _target.Connection;
      }
      set
      {
        if (value is ListeningDbConnection)
        {
          _target.Connection = ((ListeningDbConnection)value).Target;
        }
        else
        {
          _target.Connection = value;
        }
      }
    }

    public IDbDataParameter CreateParameter()
    {
      return _target.CreateParameter();
    }

    public int ExecuteNonQuery()
    {
      return _target.ExecuteNonQuery();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
      return _target.ExecuteReader(behavior);
    }

    public IDataReader ExecuteReader()
    {
      return _target.ExecuteReader();
    }

    public object ExecuteScalar()
    {
      return _target.ExecuteScalar();
    }

    public IDataParameterCollection Parameters
    {
      get { return _target.Parameters; }
    }

    public void Prepare()
    {
      _target.Prepare();
    }

    public IDbTransaction Transaction
    {
      get { return _target.Transaction; }
      set { _target.Transaction = value; }
    }

    public UpdateRowSource UpdatedRowSource
    {
      get { return _target.UpdatedRowSource; }
      set { _target.UpdatedRowSource = value; }
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
      _target.Dispose();
    }
    #endregion
  }
}