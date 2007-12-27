using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class MigrationRunnerTests : StandardFixture<MigrationRunner>
  {
    private ISchemaStateManager _schemaStateManager;
    private IMigrationFactoryChooser _migrationFactoryChooser;
    private IMigrationInitializer _migrationInitializer;
    private IMigrationFactory _migrationFactory;

    public override MigrationRunner Create()
    {
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrationFactoryChooser = _mocks.DynamicMock<IMigrationFactoryChooser>();
      _migrationInitializer = _mocks.DynamicMock<IMigrationInitializer>();
      _migrationFactory = _mocks.DynamicMock<IMigrationFactory>();
      return new MigrationRunner(_migrationFactoryChooser, _migrationInitializer, _schemaStateManager);
    }
  }
}