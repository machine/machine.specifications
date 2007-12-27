using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace Machine.Migrations.Services.Impl
{
  public class CSharpMigrationFactory : AbstractMigrationCompilerFactory, IMigrationFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CSharpMigrationFactory));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    #endregion

    #region CSharpMigrationFactory()
    public CSharpMigrationFactory(IConfiguration configuration)
    {
      _configuration = configuration;
    }
    #endregion

    #region IMigrationApplicator Members
    public IDatabaseMigration CreateMigration(MigrationReference migrationReference)
    {
      return CreateMigrationInstance(migrationReference);
    }
    #endregion

    protected override Type CompileMigration(MigrationReference migrationReference)
    {
      CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
      CompilerParameters parameters = new CompilerParameters();
      parameters.GenerateExecutable = false;
      parameters.ReferencedAssemblies.Add(typeof(IDatabaseMigration).Assembly.Location);
      foreach (string reference in _configuration.References)
      {
        _log.Debug("Referencing: " + reference);
        parameters.ReferencedAssemblies.Add(reference);
      }
      _log.InfoFormat("Compiling {0}", migrationReference);
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
      Type type = assembly.GetType(migrationReference.Name);
      if (type == null)
      {
        throw new ArgumentException("Unable to locate Migration: " + migrationReference.Path);
      }
      return type;
    }
  }
}
