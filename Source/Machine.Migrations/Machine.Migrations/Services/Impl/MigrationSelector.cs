using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationSelector : IMigrationSelector
  {
    #region Member Data
    private readonly IMigrationFinder _migrationFinder;
    private readonly IVersionStateFactory _versionStateFactory;
    #endregion

    #region MigrationSelector()
    public MigrationSelector(IMigrationFinder migrationFinder, IVersionStateFactory versionStateFactory)
    {
      _versionStateFactory = versionStateFactory;
      _migrationFinder = migrationFinder;
    }
    #endregion

    #region IMigrationSelector Members
    public ICollection<MigrationStep> SelectMigrations()
    {
      ICollection<MigrationReference> all = _migrationFinder.FindMigrations();
      List<MigrationStep> selected = new List<MigrationStep>();
      if (all.Count == 0)
      {
        return selected;
      }
      VersionState version = _versionStateFactory.CreateVersionState(all);
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
  }
}
