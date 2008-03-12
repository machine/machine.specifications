using System;
using System.Collections.Generic;
using System.Data;

using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class ConnectionProviderTests : StandardFixture<SqlServerConnectionProvider>
  {
    private IConfiguration _configuration;

    public override SqlServerConnectionProvider Create()
    {
      _configuration = _mocks.DynamicMock<IConfiguration>();
      return new SqlServerConnectionProvider(_configuration);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateConnection_Always_OpensNewSqlConnection()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.ConnectionString).Return("ConnectionString");
      }
      IDbConnection connection = _target.CurrentConnection;
      _mocks.VerifyAll();
    }
  }
}