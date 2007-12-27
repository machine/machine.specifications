using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationInitializer
  {
    void InitializeMigration(IDatabaseMigration migration);
  }
}
