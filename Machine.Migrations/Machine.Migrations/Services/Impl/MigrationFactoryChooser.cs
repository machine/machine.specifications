using System;
using System.Collections.Generic;
using System.IO;

namespace Machine.Migrations.Services.Impl
{
  public class MigrationFactoryChooser : IMigrationFactoryChooser
  {
    #region Member Data
    private readonly CSharpMigrationFactory _cSharpMigrationFactory;
    private readonly BooMigrationFactory _booMigrationFactory;
    #endregion

    #region MigrationApplicatorChooser()
    public MigrationFactoryChooser(CSharpMigrationFactory cSharpMigrationFactory, BooMigrationFactory booMigrationFactory)
    {
      _cSharpMigrationFactory = cSharpMigrationFactory;
      _booMigrationFactory = booMigrationFactory;
    }
    #endregion

    #region IMigrationApplicatorChooser Members
    public IMigrationFactory ChooseFactory(MigrationReference migrationReference)
    {
      string extension = Path.GetExtension(migrationReference.Path);
      if (extension.Equals(".cs"))
      {
        return _cSharpMigrationFactory;
      }
      if (extension.Equals(".boo"))
      {
        return _booMigrationFactory;
      }
      throw new ArgumentException(migrationReference.Path);
    }
    #endregion
  }
}
