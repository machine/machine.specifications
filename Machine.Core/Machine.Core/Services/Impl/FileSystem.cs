using System;
using System.Collections.Generic;
using System.IO;

namespace Machine.Core.Services.Impl
{
  public class FileSystem : IFileSystem
  {
    #region IFileSystem Members
    public string[] GetDirectories(string path)
    {
      return Directory.GetDirectories(path);
    }

    public string[] GetFiles(string path)
    {
      return Directory.GetFiles(path);
    }

    public bool IsFile(string path)
    {
      return File.Exists(path);
    }

    public bool IsDirectory(string path)
    {
      return Directory.Exists(path);
    }

    public Stream OpenFile(string path)
    {
      return File.OpenRead(path);
    }

    public StreamReader OpenText(string path)
    {
      return File.OpenText(path);
    }

    public Stream CreateFile(string path)
    {
      return File.Create(path);
    }

    public StreamWriter CreateText(string path)
    {
      return File.CreateText(path);
    }

    public void CopyFile(string source, string destination, bool overwrite)
    {
      File.Copy(source, destination, overwrite);
    }

    public void MoveFile(string source, string destination)
    {
      File.Move(source, destination);
    }

    public TemporaryDirectoryHandle CreateTemporaryDirectory()
    {
      string temporaryName = Path.GetTempFileName();
      string path = Path.Combine(Path.GetTempPath(), temporaryName);
      return CreateTemporaryDirectory(path);
    }

    public TemporaryDirectoryHandle CreateTemporaryDirectory(string path)
    {
      return new TemporaryDirectoryHandle(path);
    }

    public void CreateDirectory(string path)
    {
      Directory.CreateDirectory(path);
    }

    public void RemoveDirectory(string path)
    {
      Directory.Delete(path, true);
    }

    public string GetTempFileName()
    {
      return Path.GetTempFileName();
    }
    #endregion
  }
}
