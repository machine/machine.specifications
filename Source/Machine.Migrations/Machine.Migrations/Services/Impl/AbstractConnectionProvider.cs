using System.Data;

namespace Machine.Migrations.Services.Impl
{
  public abstract class AbstractConnectionProvider : IConnectionProvider
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SqlServerConnectionProvider));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    private IDbConnection _connection;
    #endregion

    #region AbstractConnectionProvider()
    public AbstractConnectionProvider(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    #endregion

    #region IConnectionProvider Members
    protected abstract IDbConnection CreateConnection(IConfiguration configuration);

    public IDbConnection OpenConnection()
    {
      return this.CurrentConnection;
    }

    public IDbConnection CurrentConnection
    {
      get
      {
        if (_connection == null)
        {
          _log.Info("Opening Connection: " + _configuration.ConnectionString);
          _connection = CreateConnection(_configuration);
          _connection.Open();
        }
        return _connection;
      }
    }
    #endregion
  }
}