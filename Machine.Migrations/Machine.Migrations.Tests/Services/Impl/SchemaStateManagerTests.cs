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
      _databaseProvider = _mocks.DynamicMock<IDatabaseProvider>();
      _schemaProvider = _mocks.DynamicMock<ISchemaProvider>();
      return new SchemaStateManager(_databaseProvider, _schemaProvider);
    }

    [Test]
    public void GetVersion_Always_JustDoesSelect()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_databaseProvider.ExecuteScalar<Int32>("SELECT {1} FROM {0}", "schema_info", "version")).Return(1);
      }
      Assert.AreEqual(1, _target.GetVersion());
      _mocks.VerifyAll();
    }

    [Test]
    public void SetVersion_Always_JustDoesUpdate()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_databaseProvider.ExecuteNonQuery("UPDATE {0} SET {1} = {2}", "schema_info", "version", 1)).Return(true);
      }
      _target.SetVersion(1);
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
        Expect.Call(_databaseProvider.ExecuteNonQuery("INSERT INTO {0} ({1}, {2}) VALUES ({3}, {4})", "schema_info", "id", "version", 1, 0)).Return(true);
      }
      _target.CheckSchemaInfoTable();
      _mocks.VerifyAll();
    }
  }
}