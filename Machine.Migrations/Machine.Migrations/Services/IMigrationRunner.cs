using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationRunner
  {
    bool CanMigrate(ICollection<MigrationReference> migrations);
    void Migrate(ICollection<MigrationReference> migrations);
  }
}
