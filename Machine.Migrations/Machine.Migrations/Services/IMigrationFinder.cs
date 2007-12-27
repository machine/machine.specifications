using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationFinder
  {
    ICollection<Migration> FindMigrations();
  }
}
