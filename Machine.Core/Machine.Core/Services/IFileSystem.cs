using System;
using System.Collections.Generic;
using System.IO;

namespace Machine.Core.Services
{
  public interface IFileSystem
  {
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
    bool IsFile(string path);
    bool IsDirectory(string path);
    Stream OpenFile(string path);
    void CopyFile(string source, string destination, bool overwrite);
  }
}
