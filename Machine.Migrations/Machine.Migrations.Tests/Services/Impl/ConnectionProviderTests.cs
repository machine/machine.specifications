using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class ConnectionProviderTests : StandardFixture<ConnectionProvider>
  {
    private IConfiguration _configuration;

    public override ConnectionProvider Create()
    {
      _configuration = _mocks.DynamicMock<IConfiguration>();
      return new ConnectionProvider(_configuration);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateConnection_Always_OpensNewSqlConnection()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.ConnectionString).Return("ConnectionString");
      }
      _target.CreateConnection();
      _mocks.VerifyAll();
    }
  }
}