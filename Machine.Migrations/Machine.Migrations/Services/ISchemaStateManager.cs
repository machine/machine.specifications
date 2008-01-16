using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface ISchemaStateManager
  {
    void CheckSchemaInfoTable();
    short[] GetAppliedMigrationVersions();
    void SetMigrationVersionUnapplied(short version);
    void SetMigrationVersionApplied(short version);
  }
}
