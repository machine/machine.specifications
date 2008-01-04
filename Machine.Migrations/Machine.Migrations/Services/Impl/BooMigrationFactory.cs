using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

namespace Machine.Migrations.Services.Impl
{
  public class BooMigrationFactory : AbstractMigrationCompilerFactory, IMigrationFactory
  {
    #region Logging
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(BooMigrationFactory));
    #endregion

    #region Member Data
    private readonly IConfiguration _configuration;
    private readonly IWorkingDirectoryManager _workingDirectoryManager;
    #endregion

    #region BooMigrationFactory()
    public BooMigrationFactory(IConfiguration configuration, IWorkingDirectoryManager workingDirectoryManager)
    {
      _configuration = configuration;
      _workingDirectoryManager = workingDirectoryManager;
    }
    #endregion

    #region IMigrationFactory Members
    public IDatabaseMigration CreateMigration(MigrationReference migrationReference)
    {
      return CreateMigrationInstance(migrationReference);
    }
    #endregion

    protected override Type CompileMigration(MigrationReference migrationReference)
    {
      BooCompiler compiler = new BooCompiler();
      compiler.Parameters.Input.Add(new FileInput(migrationReference.Path));
      compiler.Parameters.References.Add(typeof(IDatabaseMigration).Assembly);
      foreach (string reference in _configuration.References)
      {
        _log.Debug("Referencing: " + reference);
        compiler.Parameters.References.Add(Assembly.LoadFrom(reference));
      }
      compiler.Parameters.OutputType = CompilerOutputType.Library;
      compiler.Parameters.GenerateInMemory = true;
      compiler.Parameters.OutputAssembly = Path.Combine(_workingDirectoryManager.WorkingDirectory, Path.GetFileNameWithoutExtension(migrationReference.Path) + ".dll");
      compiler.Parameters.Ducky = true;
      compiler.Parameters.Pipeline = new CompileToFile();
      CompilerContext cc = compiler.Run();
      if (cc.Errors.Count > 0)
      {
        foreach (CompilerError error in cc.Errors)
        {
          _log.ErrorFormat("{0}", error);
        }
        throw new InvalidOperationException();
      }
      Assembly assembly = cc.GeneratedAssembly;
      if (assembly == null)
      {
        throw new InvalidOperationException();
      }
      Type type = cc.GeneratedAssembly.GetType(migrationReference.Name);
      if (type == null)
      {
        throw new ArgumentException("Unable to locate Migration: " + migrationReference.Path);
      }
      return type;
    }
  }
}
