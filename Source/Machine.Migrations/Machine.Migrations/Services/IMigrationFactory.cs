using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationFactory
  {
    IDatabaseMigration CreateMigration(MigrationReference migrationReference);
  }
}
