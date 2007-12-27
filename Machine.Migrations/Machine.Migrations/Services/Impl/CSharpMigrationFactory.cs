using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Machine.Migrations.Services.Impl
{
  public class CSharpMigrationFactory : IMigrationFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CSharpMigrationFactory));
    #endregion

    #region IMigrationApplicator Members
    public IDatabaseMigration CreateMigration(MigrationReference migrationReference)
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
    #endregion

    #region Methods
    private static Type CompileMigration(MigrationReference migrationReference)
    {
      CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
      CompilerParameters parameters = new CompilerParameters();
      parameters.GenerateExecutable = false;
      parameters.ReferencedAssemblies.Add(typeof(IDatabaseMigration).Assembly.Location);
      CompilerResults cr = provider.CompileAssemblyFromFile(parameters, migrationReference.Path);
      if (cr.Errors.Count > 0)
      {
        foreach (CompilerError error in cr.Errors)
        {
          _log.ErrorFormat("{0}", error);
        }
        throw new InvalidOperationException();
      }
      Assembly assembly = cr.CompiledAssembly;
      foreach (Type type in assembly.GetTypes())
      {
        return type;
      }
      throw new ArgumentException("Unable to locate Migration: " + migrationReference.Path);
    }
    #endregion
  }
}
