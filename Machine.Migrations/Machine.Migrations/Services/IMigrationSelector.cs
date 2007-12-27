using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationSelector
  {
    ICollection<MigrationStep> SelectMigrations();
  }
}
