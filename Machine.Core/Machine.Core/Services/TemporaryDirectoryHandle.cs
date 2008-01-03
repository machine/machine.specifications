using System;
using System.IO;

namespace Machine.Core.Services
{
  public class TemporaryDirectoryHandle : IDisposable
  {
    private readonly string _path;

    public string Path
    {
      get { return _path; }
    }

    public TemporaryDirectoryHandle(string path)
    {
      _path = path;
      Directory.CreateDirectory(_path);
    }

    #region IDisposable Members
    public void Dispose()
    {
      Directory.Delete(_path);
    }
    #endregion
  }
}