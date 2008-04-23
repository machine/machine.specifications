using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

using NUnit.Framework;

namespace Machine.Core.Services.Impl
{
  [TestFixture]
  public class FileSystemTests : StandardFixture<FileSystem>
  {
    private string _testDirectory;

    public override FileSystem Create()
    {
      _testDirectory = RuntimeEnvironment.GetRuntimeDirectory();
      return new FileSystem();
    }

    [Test]
    public void GetDirectories_ValidPath_ReturnsSubDirectories()
    {
      string[] actual = _target.GetDirectories(_testDirectory);
      CollectionAssert.Contains(actual, Path.Combine(_testDirectory, "CONFIG"));
    }

    [Test]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void GetDirectories_InValidPath_Throws()
    {
      _target.GetDirectories(@"C:\SomeMissingDirectory");
    }

    [Test]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void GetEntries_InValidPath_Throws()
    {
      _target.GetEntries(@"C:\SomeMissingDirectory");
    }

    [Test]
    public void GetEntries_ValidPath_ReturnsSubDirectoriesAndFiles()
    {
      string[] actual = _target.GetEntries(_testDirectory);
      CollectionAssert.Contains(actual, Path.Combine(_testDirectory, "csc.exe"));
      CollectionAssert.Contains(actual, Path.Combine(_testDirectory, "CONFIG"));
    }

    [Test]
    public void GetFiles_ValidPath_ReturnsSubFiles()
    {
      string[] actual = _target.GetFiles(_testDirectory);
      CollectionAssert.Contains(actual, Path.Combine(_testDirectory, "csc.exe"));
    }

    [Test]
    [ExpectedException(typeof(DirectoryNotFoundException))]
    public void GetFiles_InValidPath_Throws()
    {
      _target.GetFiles(@"C:\SomeMissingDirectory");
    }

    [Test]
    public void IsFile_IsAFile_IsTrue()
    {
      Assert.IsTrue(_target.IsFile(typeof(FileSystem).Assembly.Location));
    }

    [Test]
    public void GetFileProperties_IsAFile_HasProperties()
    {
      FileProperties actual = _target.GetFileProperties(typeof(FileSystem).Assembly.Location);
      Assert.Greater(actual.Length, 0);
    }

    [Test]
    public void IsFile_IsADirectory_IsFalse()
    {
      Assert.IsFalse(_target.IsFile(Path.GetDirectoryName(typeof(FileSystem).Assembly.Location)));
    }

    [Test]
    public void IsDirectory_IsAFile_IsFalse()
    {
      Assert.IsFalse(_target.IsDirectory(typeof(FileSystem).Assembly.Location));
    }

    [Test]
    public void IsDirectory_IsADirectory_IsTrue()
    {
      Assert.IsTrue(_target.IsDirectory(Path.GetDirectoryName(typeof(FileSystem).Assembly.Location)));
    }

    [Test]
    public void OpenFile_IsFile_GetsStream()
    {
      using (Stream stream = _target.OpenFile(typeof(FileSystem).Assembly.Location))
      {
      }
    }

    [Test]
    public void OpenText_IsFile_GetsStream()
    {
      using (StreamReader stream = _target.OpenText(typeof(FileSystem).Assembly.Location))
      {
      }
    }

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void OpenFile_IsNotFile_Throws()
    {
      _target.OpenFile(@"C:\NotAFile");
    }

    [Test]
    public void CreateTemporaryDirectory_Always_MakesNewDirectory()
    {
      TemporaryDirectoryHandle handle = _target.CreateTemporaryDirectory("abcdefg");
      Console.WriteLine("{0}", handle.Path);
      Assert.IsTrue(Directory.Exists(handle.Path));
      handle.Dispose();
      Assert.IsFalse(Directory.Exists(handle.Path));
    }
  }
}
