using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationRunner
  {
    bool CanMigrate(ICollection<MigrationStep> steps);
    void Migrate(ICollection<MigrationStep> steps);
  }
}
