using System;
using System.CodeDom.Compiler;
using System.Data.SqlTypes;
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
    private readonly IWorkingDirectoryManager _workingDirectoryManager;
    #endregion

    #region CSharpMigrationFactory()
    public CSharpMigrationFactory(IConfiguration configuration, IFileSystem fileSystem, IWorkingDirectoryManager workingDirectoryManager)
    {
      _configuration = configuration;
      _workingDirectoryManager = workingDirectoryManager;
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
      parameters.OutputAssembly = Path.Combine(_workingDirectoryManager.WorkingDirectory, Path.GetFileNameWithoutExtension(migrationReference.Path) + ".dll");
      parameters.ReferencedAssemblies.Add(typeof(IDatabaseMigration).Assembly.Location);
      parameters.ReferencedAssemblies.Add(typeof(SqlMoney).Assembly.Location);
      parameters.IncludeDebugInformation = true;
      foreach (string reference in _configuration.References)
      {
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
        throw new ArgumentException("Unable to locate Migration: " + migrationReference.Name);
      }
      return type;
    }
  }
}
