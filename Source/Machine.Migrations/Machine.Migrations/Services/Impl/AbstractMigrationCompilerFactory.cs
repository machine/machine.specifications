using System;

namespace Machine.Migrations.Services.Impl
{
  public abstract class AbstractMigrationCompilerFactory
  {
    protected IDatabaseMigration CreateMigrationInstance(MigrationReference migrationReference)
    {
      Type type = CompileMigration(migrationReference);
      object fixture = Activator.CreateInstance(type);
      IDatabaseMigration instance = fixture as IDatabaseMigration;
      if (instance == null)
      {
        throw new ArgumentException(type + " should be a " + typeof(IDatabaseMigration));
      }
      return instance;
    }

    protected abstract Type CompileMigration(MigrationReference migrationReference);
  }
}