using System;
using System.Collections.Generic;
using Castle.Windsor;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Machine.IoC;
using Machine.Core.MsBuildUtilities;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;
using Machine.Migrations.Services.Impl;
using Machine.Core.Services.Impl;
using Machine.Core.Services;
using Machine.Migrations.Services;

namespace Machine.Migrations
{
  public class MigratorTask : Task, IConfiguration
  {
    private string _migrationsDirectory;
    private string _connectionString;
    private short _desiredVersion;
    private bool _diagnostics;
    private string[] _references;

    public MigratorTask()
    {
      _migrationsDirectory = Environment.CurrentDirectory;
    }

    public virtual IWindsorContainer CreateContainer()
    {
      return new WindsorContainer();
    }

    public virtual IWindsorContainer CreateAndPopulateContainer()
    {
      IWindsorContainer windsor = CreateContainer();
      WindsorWrapper container = new WindsorWrapper(windsor);
      container.AddService<IConnectionProvider>(this.ConnectionProviderType);
      container.AddService<ITransactionProvider>(this.TransactionManagerType);
      container.AddService<IFileSystem, FileSystem>();
      container.AddService<INamer, Namer>();
      container.AddService<ISchemaStateManager, SchemaStateManager>();
      container.AddService<IMigrationFinder, MigrationFinder>();
      container.AddService<IMigrationSelector, MigrationSelector>();
      container.AddService<IMigrationRunner, MigrationRunner>();
      container.AddService<IMigrationInitializer, MigrationInitializer>();
      container.AddService<IDatabaseProvider, SqlServerDatabaseProvider>();
      container.AddService<ISchemaProvider, SqlServerSchemaProvider>();
      container.AddService<IMigrator, Migrator>();
      container.AddService<IMigrationFactoryChooser, MigrationFactoryChooser>();
      container.AddService<IVersionStateFactory, VersionStateFactory>();
      container.AddService<IWorkingDirectoryManager, WorkingDirectoryManager>();
      container.AddService<ICommonTransformations, CommonTransformations>();
      container.AddService<IConfiguration>(this);
      container.AddService<CSharpMigrationFactory>();
      container.AddService<BooMigrationFactory>();
      return windsor;
    }

    public override bool Execute()
    {
      log4net.Config.BasicConfigurator.Configure(new Log4NetMsBuildAppender(this.Log, new log4net.Layout.PatternLayout("%-5p %x %m")));
      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push(String.Empty))
      {
        IWindsorContainer container = CreateAndPopulateContainer();
        container.Resolve<IMigrator>().RunMigrator();
      }
      return true;
    }

    #region IConfiguration Members
    [Required]
    public string ConnectionString
    {
      get { return _connectionString; }
      set { _connectionString = value; }
    }

    public virtual Type ConnectionProviderType
    {
      get { return typeof(ConnectionProvider); }
    }

    public virtual Type TransactionManagerType
    {
      get { return typeof(TransactionProvider); }
    }

    public string MigrationsDirectory
    {
      get { return _migrationsDirectory; }
      set { _migrationsDirectory = value; }
    }

    public short DesiredVersion
    {
      get { return _desiredVersion; }
      set { _desiredVersion = value; }
    }

    public bool ShowDiagnostics
    {
      get { return _diagnostics; }
      set { _diagnostics = value; }
    }

    public string[] References
    {
      get
      {
        if (_references == null)
        {
          return new string[0];
        }
        return _references;
      }
      set { _references = value; }
    }
    #endregion
  }
}
