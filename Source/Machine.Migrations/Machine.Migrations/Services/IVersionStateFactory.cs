using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IVersionStateFactory
  {
    VersionState CreateVersionState(ICollection<MigrationReference> migrations);
  }
}
