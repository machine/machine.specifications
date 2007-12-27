using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface ISchemaStateManager
  {
    void CheckSchemaInfoTable();
    short GetVersion();
    void SetVersion(short version);
  }
}
