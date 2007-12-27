using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using StructureMap;
using StructureMap.Configuration.DSL;

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

    public override bool Execute()
    {
      log4net.Config.BasicConfigurator.Configure(new Log4NetMsBuildAppender(this.Log, new log4net.Layout.PatternLayout("%-5p %x %m")));

      StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;  
      StructureMapConfiguration.BuildInstancesOf<IFileSystem>().TheDefaultIsConcreteType<FileSystem>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<INamer>().TheDefaultIsConcreteType<Namer>().AsSingletons();

      StructureMapConfiguration.BuildInstancesOf<ISchemaStateManager>().TheDefaultIsConcreteType<SchemaStateManager>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrationFinder>().TheDefaultIsConcreteType<MigrationFinder>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrationSelector>().TheDefaultIsConcreteType<MigrationSelector>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrationRunner>().TheDefaultIsConcreteType<MigrationRunner>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrationInitializer>().TheDefaultIsConcreteType<MigrationInitializer>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IDatabaseProvider>().TheDefaultIsConcreteType<SqlServerDatabaseProvider>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<ISchemaProvider>().TheDefaultIsConcreteType<SqlServerSchemaProvider>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrator>().TheDefaultIsConcreteType<Migrator>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IMigrationFactoryChooser>().TheDefaultIsConcreteType<MigrationFactoryChooser>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IVersionStateFactory>().TheDefaultIsConcreteType<VersionStateFactory>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<IConfiguration>().TheDefaultIs(Registry.Object(this)).AsSingletons();

      StructureMapConfiguration.BuildInstancesOf<CSharpMigrationFactory>().TheDefaultIsConcreteType<CSharpMigrationFactory>().AsSingletons();
      StructureMapConfiguration.BuildInstancesOf<BooMigrationFactory>().TheDefaultIsConcreteType<BooMigrationFactory>().AsSingletons();

      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push(String.Empty))
      {
        ObjectFactory.GetInstance<IMigrator>().RunMigrator();
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
