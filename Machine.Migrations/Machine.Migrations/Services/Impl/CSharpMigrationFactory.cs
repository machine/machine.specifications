using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Machine.Core.Services;

namespace Machine.Migrations.Services.Impl
{
  public class CSharpMigrationFactory : AbstractMigrationCompilerFactory, IMigrationFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CSharpMigrationFactory));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    private readonly IFileSystem _fileSystem;
    #endregion

    #region CSharpMigrationFactory()
    public CSharpMigrationFactory(IConfiguration configuration, IFileSystem fileSystem)
    {
      _configuration = configuration;
      _fileSystem = fileSystem;
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
        // _log.Debug("Referencing: " + reference);
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
      foreach (string reference in _configuration.References)
      {
        if (_fileSystem.IsFile(reference))
        {
          _fileSystem.CopyFile(reference, Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileName(reference)), true);
        }
      }
      Type type = assembly.GetType(migrationReference.Name);
      foreach (Type possiblyAMigrationType in assembly.GetExportedTypes())
      {
        _log.InfoFormat("Exported: {0}", possiblyAMigrationType);
      }
      if (type == null)
      {
        throw new ArgumentException("Unable to locate Migration: " + migrationReference.Name);
      }
      return type;
    }
  }
}
