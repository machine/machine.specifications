using System;
using System.Collections.Generic;
using System.Data;

namespace Machine.Migrations.Services
{
  public interface IConnectionProvider
  {
    IDbConnection CreateConnection();
  }
}
