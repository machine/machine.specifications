using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.NHibernateExtensions.Listening
{
  public class ListeningDbConnection : IDbConnection
  {
    #region Member Data
    private readonly IDbConnection _target;
    #endregion

    #region Properties
    public IDbConnection Target
    {
      get { return _target; }
    }
    #endregion

    #region ListeningDbConnection()
    public ListeningDbConnection(IDbConnection target)
    {
      _target = target;
    }
    #endregion

    #region IDbConnection Members
    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
      return _target.BeginTransaction(il);
    }

    public IDbTransaction BeginTransaction()
    {
      return _target.BeginTransaction();
    }

    public void ChangeDatabase(string databaseName)
    {
      _target.ChangeDatabase(databaseName);
    }

    public void Close()
    {
      _target.Close();
    }

    public string ConnectionString
    {
      get { return _target.ConnectionString; }
      set { _target.ConnectionString = value; }
    }

    public int ConnectionTimeout
    {
      get { return _target.ConnectionTimeout; }
    }

    public IDbCommand CreateCommand()
    {
      return new ListeningDbCommand(_target.CreateCommand());
    }

    public string Database
    {
      get { return _target.Database; }
    }

    public void Open()
    {
      _target.Open();
    }

    public ConnectionState State
    {
      get { return _target.State; }
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