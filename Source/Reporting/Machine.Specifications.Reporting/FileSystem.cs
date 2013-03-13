using System;
using System.IO;

namespace Machine.Specifications.Reporting
{
  public interface IFileSystem
  {
    bool IsValidPathToDirectory(string path);
    void EnsureDirectoryExists(string path);
    bool IsValidPathToFile(string path);
    void DeleteIfFileExists(string path);
    void Move(string source, string destination);
  }

  internal class FileSystem : IFileSystem
  {
    public bool IsValidPathToDirectory(string path)
    {
      try
      {
        return Directory.Exists(path);
      }
      catch (NullReferenceException)
      {
        return false;
      }
    }

    public void EnsureDirectoryExists(string path)
    {
      if (Directory.Exists(path))
      {
        return;
      }

      Directory.CreateDirectory(path);
    }

    public bool IsValidPathToFile(string path)
    {
      return IsValidPathToDirectory(new FileInfo(path).DirectoryName);
    }

    public void DeleteIfFileExists(string path)
    {
      if (File.Exists(path))
      {
        File.Delete(path);
      }
    }

    public void Move(string source, string destination)
    {
      File.Move(source, destination);
    }
  }
}