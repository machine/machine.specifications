using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IConfiguration
  {
    Type ConnectionProviderType
    {
      get;
    }

    Type TransactionManagerType
    {
      get;
    }
    string ConnectionString
    {
      get;
    }

    string MigrationsDirectory
    {
      get;
    }

    short DesiredVersion
    {
      get;
    }

    bool ShowDiagnostics
    {
      get;
    }

    string[] References
    {
      get;
    }
  }
}
