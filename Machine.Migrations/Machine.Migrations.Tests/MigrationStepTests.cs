using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations
{
  [TestFixture]
  public class MigrationStepTests
  {
    private MigrationReference _reference1;
    private MigrationReference _reference2;
    private MigrationStep _step1;
    private MigrationStep _step2;
    private MockRepository _mocks;

    [Test]
    public void Equals_SameReference_SameReverting_IsTrue()
    {
      _step1 = new MigrationStep(_reference1, true);
      _step2 = new MigrationStep(_reference1, true);
      Assert.IsTrue(_step1.Equals(_step2));
    }

    [Test]
    public void Equals_SameReference_DifferentReverting_IsFalse()
    {
      _step1 = new MigrationStep(_reference1, true);
      _step2 = new MigrationStep(_reference1, false);
      Assert.IsFalse(_step1.Equals(_step2));
    }

    [Test]
    public void Equals_DifferentReference_SameReverting_IsFalse()
    {
      _step1 = new MigrationStep(_reference1, true);
      _step2 = new MigrationStep(_reference2, true);
      Assert.IsFalse(_step1.Equals(_step2));
    }

    [Test]
    public void Equals_DifferentType_IsFalse()
    {
      _step1 = new MigrationStep(_reference1, true);
      Assert.IsFalse(_step1.Equals("HI"));
    }

    [Test]
    public void GetHashCode_Always_SameAsReferenceThatIsGoodEnough()
    {
      _step1 = new MigrationStep(_reference1, true);
      Assert.AreEqual(_reference1.GetHashCode(), _step1.GetHashCode());
    }

    [Test]
    public void Apply_WhenNotReverting_GoesUp()
    {
      _step1 = new MigrationStep(_reference1, false);
      _step1.DatabaseMigration = _mocks.CreateMock<IDatabaseMigration>();
      using (_mocks.Record())
      {
        _step1.DatabaseMigration.Up();
      }
      _step1.Apply();
      _mocks.VerifyAll();
    }

    [Test]
    public void Apply_WhenReverting_GoesDown()
    {
      _step1 = new MigrationStep(_reference1, true);
      _step1.DatabaseMigration = _mocks.CreateMock<IDatabaseMigration>();
      using (_mocks.Record())
      {
        _step1.DatabaseMigration.Down();
      }
      _step1.Apply();
      _mocks.VerifyAll();
    }

    [Test]
    public void VersionAfterApplication_WhenNotReverting_IsOurVersion()
    {
      _step1 = new MigrationStep(_reference1, false);
      Assert.AreEqual(1, _step1.VersionAfterApplying);
    }

    [Test]
    public void VersionAfterApplication_WhenReverting_IsPreviousVersion()
    {
      _step1 = new MigrationStep(_reference1, true);
      Assert.AreEqual(0, _step1.VersionAfterApplying);
    }

    [SetUp]
    public void Setup()
    {
      _reference1 = new MigrationReference(1, "A", "001_a.cs");
      _reference2 = new MigrationReference(2, "B", "002_b.cs");
      _mocks = new MockRepository();
    }
  }
}