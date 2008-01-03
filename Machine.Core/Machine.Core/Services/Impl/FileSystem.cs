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

    public void CopyFile(string source, string destination, bool overwrite)
    {
      File.Copy(source, destination, overwrite);
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
    #endregion
  }
}
