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
    private IMigrationFactory _migrationFactory;
    private IMigrationFactoryChooser _migrationFactoryChooser;

    public override MigrationRunner Create()
    {
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrationFactoryChooser = _mocks.DynamicMock<IMigrationFactoryChooser>();
      _migrationFactory = _mocks.DynamicMock<IMigrationFactory>();
      return new MigrationRunner(_schemaStateManager, _migrationFactoryChooser);
    }
  }
}