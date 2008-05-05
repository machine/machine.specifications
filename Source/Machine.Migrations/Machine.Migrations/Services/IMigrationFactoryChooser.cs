using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationFactoryChooser
  {
    IMigrationFactory ChooseFactory(MigrationReference migrationReference);
  }
}
