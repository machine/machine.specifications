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
  public class CommonTransformationsTests : StandardFixture<CommonTransformations>
  {
    private IDatabaseProvider _databaseProvider;
    private ISchemaProvider _schemaProvider;

    public override CommonTransformations Create()
    {
      _databaseProvider = _mocks.CreateMock<IDatabaseProvider>();
      _schemaProvider = _mocks.CreateMock<ISchemaProvider>();
      return new CommonTransformations(_databaseProvider, _schemaProvider);
    }

    [Test]
    public void AddColumn_WithCurrentValues_AddsAndUpdatesAndAlters()
    {
      using (_mocks.Record())
      {
        _schemaProvider.AddColumn("TheTable", "TheColumn", typeof(Int32), 0, false, true);
        Expect.Call(_databaseProvider.ExecuteNonQuery("UPDATE {0} SET {1} = '{2}'", "TheTable", "TheColumn", 1)).Return(true);
        _schemaProvider.ChangeColumn("TheTable", "TheColumn", typeof(Int32), 0, false);
      }
      _target.AddColumn("TheTable", "TheColumn", typeof(Int32), 0, false, 1);
      _mocks.VerifyAll();
    }
  }
}