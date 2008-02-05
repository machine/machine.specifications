using System;
using System.Collections.Generic;
using Castle.Windsor;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Machine.Core.MsBuildUtilities;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;
using Machine.Migrations.Services.Impl;
using Machine.Core.Services.Impl;
using Machine.Core.Services;
using Machine.Migrations.Services;

namespace Machine.Migrations
{
  public class MachineContainer : WindsorContainer
  {
    public void AddService<TService>(Type implementation)
    {
      AddComponent(MakeKey(implementation), typeof(TService), implementation);
    }

    public void AddService<TService, TImpl>() where TImpl : TService
    {
      AddService<TService>(typeof(TImpl));
    }

    public void AddService<TService>(TService implementation)
    {
      Kernel.AddComponentInstance(MakeKey(implementation.GetType()), typeof(TService), implementation);
    }

    public void AddService<TImpl>()
    {
      AddComponent(MakeKey(typeof(TImpl)), typeof(TImpl));
    }

    private static string MakeKey(Type implementation)
    {
      return implementation.FullName;
    }
  }
  public class MigratorTask : Task, IConfiguration
  {
    private string _migrationsDirectory;
    private string _connectionString;
    private short _desiredVersion;
    private bool _diagnostics;
    private string[] _references;
    private string _connectionProvider;
    private string _transactionManager;

    public MigratorTask()
    {
      _migrationsDirectory = Environment.CurrentDirectory;
    }

    public override bool Execute()
    {
      log4net.Config.BasicConfigurator.Configure(new Log4NetMsBuildAppender(this.Log, new log4net.Layout.PatternLayout("%-5p %x %m")));

      MachineContainer container = new MachineContainer();
      container.AddService<IConnectionProvider>(this.ConnectionProviderType);
      container.AddService<IMigrationTransactionService>(this.TransactionManagerType);
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

      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push(String.Empty))
      {
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

    public Type ConnectionProviderType
    {
      get
      {
        if (String.IsNullOrEmpty(_connectionProvider))
        {
          return typeof(ConnectionProvider);
        }
        return Type.GetType(_connectionProvider);
      }
    }

    public string ConnectionProvider
    {
      get { return _connectionProvider; }
      set { _connectionProvider = value; }
    }

    public Type TransactionManagerType
    {
      get
      {
        if (String.IsNullOrEmpty(_transactionManager))
        {
          return typeof(MigrationTransactionService);
        }
        return Type.GetType(_transactionManager);
      }
    }

    public string TransactionManager
    {
      get { return _transactionManager; }
      set { _transactionManager = value; }
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
