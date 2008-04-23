using System;
using System.Collections.Generic;
using System.IO;

using Machine.Core.Services;

namespace Machine.Core.Services
{
  public interface IFileSystem
  {
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
    string[] GetEntries(string path);
    bool IsFile(string path);
    bool IsDirectory(string path);
    FileProperties GetFileProperties(string path);
    Stream OpenFile(string path);
    StreamReader OpenText(string path);
    Stream CreateFile(string path);
    StreamWriter CreateText(string path);
    void CopyFile(string source, string destination, bool overwrite);
    TemporaryDirectoryHandle CreateTemporaryDirectory();
    TemporaryDirectoryHandle CreateTemporaryDirectory(string path);
    void CreateDirectory(string path);
    void RemoveDirectory(string path);
    string GetTempFileName();
    void MoveFile(string source, string destination);
  }
  public class FileProperties
  {
    private string _fullName;
    private long _length;
    private DateTime _lastAccessTime;
    private DateTime _lastAccessTimeUtc;
    private DateTime _lastWriteTime;
    private DateTime _lastWriteTimeUtc;
    private DateTime _creationTime;
    private DateTime _creationTimeUtc;

    public string FullName
    {
      get { return _fullName; }
      set { _fullName = value; }
    }

    public long Length
    {
      get { return _length; }
      set { _length = value; }
    }

    public DateTime LastAccessTime
    {
      get { return _lastAccessTime; }
      set { _lastAccessTime = value; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _lastAccessTimeUtc; }
      set { _lastAccessTimeUtc = value; }
    }

    public DateTime LastWriteTime
    {
      get { return _lastWriteTime; }
      set { _lastWriteTime = value; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _lastWriteTimeUtc; }
      set { _lastWriteTimeUtc = value; }
    }

    public DateTime CreationTime
    {
      get { return _creationTime; }
      set { _creationTime = value; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _creationTimeUtc; }
      set { _creationTimeUtc = value; }
    }

    public FileProperties(FileInfo fileInfo)
    {
      _fullName = fileInfo.FullName;
      _length = fileInfo.Length;
      _lastAccessTime = fileInfo.LastAccessTime;
      _lastAccessTimeUtc = fileInfo.LastAccessTimeUtc;
      _lastWriteTime = fileInfo.LastWriteTime;
      _lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
      _creationTime = fileInfo.CreationTime;
      _creationTimeUtc = fileInfo.CreationTimeUtc;
    }

    public FileProperties()
    {
    }
  }
}
