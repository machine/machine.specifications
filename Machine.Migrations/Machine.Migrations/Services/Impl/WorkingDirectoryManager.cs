using System;
using System.Collections.Generic;
using System.IO;

using Machine.Core.Services;

namespace Machine.Migrations.Services.Impl
{
  public class WorkingDirectoryManager : IWorkingDirectoryManager
  {
    private readonly IFileSystem _fileSystem;
    private readonly IConfiguration _configuration;

    public WorkingDirectoryManager(IFileSystem fileSystem, IConfiguration configuration)
    {
      _fileSystem = fileSystem;
      _configuration = configuration;
    }

    #region IWorkingDirectoryManager Members
    public string WorkingDirectory
    {
      get
      {
        return Path.Combine(_configuration.MigrationsDirectory, "WorkingTemp");
      }
    }

    public void Create()
    {
      _fileSystem.CreateDirectory(this.WorkingDirectory);
      foreach (string reference in _configuration.References)
      {
        if (_fileSystem.IsFile(reference))
        {
          _fileSystem.CopyFile(reference, Path.Combine(this.WorkingDirectory, Path.GetFileName(reference)), true);
        }
      }
    }

    public void Destroy()
    {
      _fileSystem.RemoveDirectory(this.WorkingDirectory);
    }
    #endregion
  }
}
