using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class SchemaStateManagerTests : StandardFixture<SchemaStateManager>
  {
    private IDatabaseProvider _databaseProvider;
    private ISchemaProvider _schemaProvider;

    public override SchemaStateManager Create()
    {
      _databaseProvider = _mocks.CreateMock<IDatabaseProvider>();
      _schemaProvider = _mocks.DynamicMock<ISchemaProvider>();
      return new SchemaStateManager(_databaseProvider, _schemaProvider);
    }

    [Test]
    public void GetAppliedMigrationVersions_Always_JustDoesSelectAndReturnsArray()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_databaseProvider.ExecuteScalarArray<Int16>("SELECT CAST({1} AS SMALLINT) FROM {0} ORDER BY {1}", "schema_info", "version")).Return(new Int16[] { 1, 2, 3 });
      }
      Assert.AreEqual(new Int16[] { 1, 2, 3 }, _target.GetAppliedMigrationVersions());
      _mocks.VerifyAll();
    }

    [Test]
    public void CheckSchemaInfoTable_DoesHaveTable_DoesNothing()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_schemaProvider.HasTable("schema_info")).Return(true);
      }
      _target.CheckSchemaInfoTable();
      _mocks.VerifyAll();
    }

    [Test]
    public void CheckSchemaInfoTable_DoesNotHaveTable_CreatesTable()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_schemaProvider.HasTable("schema_info")).Return(false);
        _schemaProvider.AddTable("schema_info", null);
        LastCall.IgnoreArguments();
      }
      _target.CheckSchemaInfoTable();
      _mocks.VerifyAll();
    }

    [Test]
    public void SetMigrationVersionUnapplied_Always_NukesRow()
    {
      short version = 1;
      using (_mocks.Record())
      {
        SetupResult.For(_databaseProvider.ExecuteNonQuery("DELETE FROM {0} WHERE {1} = {2}", "schema_info", "version", version)).Return(true);
      }
      _target.SetMigrationVersionUnapplied(version);
      _mocks.VerifyAll();
    }

    [Test]
    public void SetMigrationVersionApplied_Always_AddsRow()
    {
      short version = 2;
      using (_mocks.Record())
      {
        SetupResult.For(_databaseProvider.ExecuteNonQuery("INSERT INTO {0} ({2}) VALUES ({4})", "schema_info", "id", "version", version, version)).Return(true);
      }
      _target.SetMigrationVersionApplied(version);
      _mocks.VerifyAll();
    }
  }
}