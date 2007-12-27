using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

using NUnit.Framework;

namespace Machine.Migrations
{
  [TestFixture]
  public class SimpleMigrationTests : StandardFixture<ConcreteSimpleMigration>
  {
    private IDatabaseProvider _databaseProvider;
    private ISchemaProvider _schemaProvider;

    public override ConcreteSimpleMigration Create()
    {
      _databaseProvider = _mocks.DynamicMock<IDatabaseProvider>();
      _schemaProvider = _mocks.DynamicMock<ISchemaProvider>();
      return new ConcreteSimpleMigration();
    }

    [Test]
    public void Initialize_Always_SetsServices()
    {
      _target.Initialize(_databaseProvider, _schemaProvider);
      Assert.AreEqual(_databaseProvider, _target.Database);
      Assert.AreEqual(_schemaProvider, _target.Schema);
    }
  }

  public class ConcreteSimpleMigration : SimpleMigration
  {
    public override void Up()
    {
    }

    public override void Down()
    {
    }
  }
}