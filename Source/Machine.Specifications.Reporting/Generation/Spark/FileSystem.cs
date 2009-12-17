using System;
using System.IO;

namespace Machine.Specifications.Reporting.Generation.Spark
{
  public interface IFileSystem
  {
    bool IsValidPathToDirectory(string path);
    void CreateOrOverwriteDirectory(string path);
    bool IsValidPathToFile(string path);
    void DeleteIfFileExists(string path);
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

    public void CreateOrOverwriteDirectory(string path)
    {
      if (Directory.Exists(path))
      {
        Directory.Delete(path, true);
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
  }
}