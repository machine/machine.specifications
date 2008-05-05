using System;

namespace Machine.Migrations.Services
{
  public interface IWorkingDirectoryManager
  {
    string WorkingDirectory
    {
      get;
    }
    void Create();
    void Destroy();
  }
}
