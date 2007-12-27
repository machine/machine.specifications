using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationRunner
  {
    bool CanMigrate(ICollection<Migration> migrations);
    void Migrate(ICollection<Migration> migrations);
  }
}
