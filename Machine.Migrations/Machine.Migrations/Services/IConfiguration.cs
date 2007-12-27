using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IConfiguration
  {
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
  }
}
