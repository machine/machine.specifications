using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using Machine.Core.Services;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationFinder : IMigrationFinder
  {
    #region Member Data
    private readonly Regex _regex = new Regex(@"^(\d+)_([\w_]+)\.(cs|boo)$");
    private readonly IConfiguration _configuration;
    private readonly IFileSystem _fileSystem;
    #endregion

    #region MigrationFinder()
    public MigrationFinder(IFileSystem fileSystem, IConfiguration configuration)
    {
      _fileSystem = fileSystem;
      _configuration = configuration;
    }
    #endregion

    #region IMigrationFinder Members
    public ICollection<MigrationReference> FindMigrations()
    {
      List<MigrationReference> migrations = new List<MigrationReference>();
      foreach (string file in _fileSystem.GetFiles(_configuration.MigrationsDirectory))
      {
        Match m = _regex.Match(Path.GetFileName(file));
        if (m.Success)
        {
          migrations.Add(new MigrationReference(Int16.Parse(m.Groups[1].Value), m.Groups[2].Value, file));
        }
      }
      return migrations;
    }
    #endregion
  }
}
