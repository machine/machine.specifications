using System;
using System.Collections.Generic;

using Machine.Core;
using Machine.Core.Services;

using NUnit.Framework;
using Rhino.Mocks;

namespace Machine.Migrations.Services.Impl
{
  [TestFixture]
  public class WorkingDirectoryManagerTests : StandardFixture<WorkingDirectoryManager>
  {
    private IFileSystem _fileSystem;
    private IConfiguration _configuration;

    [Test]
    public void Create_Always_CreatesDirectory()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
        _fileSystem.CreateDirectory(@"Migrations\WorkingTemp");
        SetupResult.For(_configuration.References).Return(new string[0]);
      }
      _target.Create();
      _mocks.VerifyAll();
    }

    [Test]
    public void Create_WithFileReferences_CopiesThem()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
        _fileSystem.CreateDirectory(@"Migrations\WorkingTemp");
        SetupResult.For(_configuration.References).Return(new string[] { "AFile" });
        SetupResult.For(_fileSystem.IsFile("AFile")).Return(true);
        _fileSystem.CopyFile("AFile", @"Migrations\WorkingTemp\AFile", true);
      }
      _target.Create();
      _mocks.VerifyAll();
    }

    [Test]
    public void Create_WithNonFileReferences_LeavesThemAlone()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
        _fileSystem.CreateDirectory(@"Migrations\WorkingTemp");
        SetupResult.For(_configuration.References).Return(new string[] { "AGacedOne" });
        SetupResult.For(_fileSystem.IsFile("AGacedOne")).Return(false);
      }
      _target.Create();
      _mocks.VerifyAll();
    }

    [Test]
    public void Destroy_DirectoryDoesNotExist_DoesntDestroy()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
        SetupResult.For(_fileSystem.IsDirectory(@"Migrations\WorkingTemp")).Return(false);
        _fileSystem.RemoveDirectory(@"Migrations\WorkingTemp");
        LastCall.IgnoreArguments().Repeat.Never();
      }
      _target.Destroy();
      _mocks.VerifyAll();
    }

    [Test]
    public void Destroy_DirectoryExists_DestroysDirectory()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
        SetupResult.For(_fileSystem.IsDirectory(@"Migrations\WorkingTemp")).Return(true);
        _fileSystem.RemoveDirectory(@"Migrations\WorkingTemp");
      }
      _target.Destroy();
      _mocks.VerifyAll();
    }

    [Test]
    public void GetWorkingDirectory_Always_IsPath()
    {
      using (_mocks.Record())
      {
        SetupResult.For(_configuration.MigrationsDirectory).Return("Migrations");
      }
      Assert.IsFalse(String.IsNullOrEmpty(_target.WorkingDirectory));
    }

    public override WorkingDirectoryManager Create()
    {
      _fileSystem = _mocks.DynamicMock<IFileSystem>();
      _configuration = _mocks.DynamicMock<IConfiguration>();
      return new WorkingDirectoryManager(_fileSystem, _configuration);
    }
  }
}