using System;
using System.Collections.Generic;

using Machine.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class VersionStateFactoryTests : StandardFixture<VersionStateFactory>
  {
    private IConfiguration _configuration;
    private ISchemaStateManager _schemaStateManager;
    private List<MigrationReference> _migrations;

    public override VersionStateFactory Create()
    {
      _configuration = _mocks.DynamicMock<IConfiguration>();
      _schemaStateManager = _mocks.DynamicMock<ISchemaStateManager>();
      _migrations = new List<MigrationReference>();
      _migrations.Add(new MigrationReference(1, "A", "001_a.cs"));
      _migrations.Add(new MigrationReference(2, "B", "002_b.cs"));
      _migrations.Add(new MigrationReference(3, "C", "003_c.cs"));
      _migrations.Add(new MigrationReference(4, "D", "004_d.cs"));
      return new VersionStateFactory(_configuration, _schemaStateManager);
    }

    [Test]
    public void CreateVersionState_Always_SetsApplied()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.DesiredVersion).Return((short)2);
        SetupResult.For(_schemaStateManager.GetAppliedMigrationVersions()).Return(new short[] { 1, 2, 3});
      }
      VersionState actual = _target.CreateVersionState(_migrations);
      CollectionAssert.AreEqual(new short[] { 1, 2, 3 }, new List<short>(actual.Applied));
    }

    [Test]
    public void CreateVersionState_Always_SetsLast()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.DesiredVersion).Return((short)2);
        SetupResult.For(_schemaStateManager.GetVersion()).Return((short)3);
      }
      VersionState actual = _target.CreateVersionState(_migrations);
      Assert.AreEqual(4, actual.Last);
    }

    [Test]
    public void CreateVersionState_DesiredGreaterLessThanZero_SetsToLast()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.DesiredVersion).Return((short)-1);
        SetupResult.For(_schemaStateManager.GetVersion()).Return((short)3);
      }
      VersionState actual = _target.CreateVersionState(_migrations);
      Assert.AreEqual(4, actual.Desired);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateVersionState_DesiredGreaterThanLast_Throws()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.DesiredVersion).Return((short)5);
        SetupResult.For(_schemaStateManager.GetVersion()).Return((short)3);
      }
      _target.CreateVersionState(_migrations);
    }
  }
}