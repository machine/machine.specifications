using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationSelector : IMigrationSelector
  {
    #region Member Data
    private readonly IConfiguration _configuration;
    private readonly ISchemaStateManager _schemaStateManager;
    private readonly IMigrationFinder _migrationFinder;
    #endregion

    #region MigrationSelector()
    public MigrationSelector(ISchemaStateManager schemaStateManager, IMigrationFinder migrationFinder, IConfiguration configuration)
    {
      _schemaStateManager = schemaStateManager;
      _configuration = configuration;
      _migrationFinder = migrationFinder;
    }
    #endregion

    #region IMigrationSelector Members
    public ICollection<MigrationStep> SelectMigrations()
    {
      List<MigrationReference> all = new List<MigrationReference>(_migrationFinder.FindMigrations());
      List<MigrationStep> selected = new List<MigrationStep>();
      if (all.Count == 0)
      {
        return selected;
      }
      VersionState version = GetVersionState(all);
      foreach (MigrationReference migration in all)
      {
        if (version.IsApplicable(migration))
        {
          MigrationStep step = new MigrationStep(migration, version.IsReverting);
          selected.Add(step);
        }
      }
      if (version.IsReverting)
      {
        selected.Reverse();
      }
      return selected;
    }
    #endregion

    private VersionState GetVersionState(ICollection<MigrationReference> migrations)
    {
      short current = _schemaStateManager.GetVersion();
      short desired = _configuration.DesiredVersion;
      short last = 0;
      if (migrations.Count > 0)
      {
        List<MigrationReference> all = new List<MigrationReference>(migrations);
        last = all[all.Count - 1].Version;
      }
      if (desired > last)
      {
        throw new ArgumentException("DesiredVersion is greater than maximum migration!");
      }
      if (desired < 0)
      {
        desired = last;
      }
      return new VersionState(current, last, desired);
    }
  }
}
