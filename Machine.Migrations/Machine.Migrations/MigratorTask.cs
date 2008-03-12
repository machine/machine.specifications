using System;
using System.Collections.Generic;
using Castle.Windsor;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Machine.Core.MsBuildUtilities;
using Machine.Migrations.Services.Impl;
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

    public virtual IMigratorContainerFactory CreateContainerFactory()
    {
      return new MigratorContainerFactory();
    }

    public override bool Execute()
    {
      IMigratorContainerFactory migratorContainerFactory = CreateContainerFactory();
      log4net.Config.BasicConfigurator.Configure(new Log4NetMsBuildAppender(this.Log, new log4net.Layout.PatternLayout("%-5p %x %m")));
      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push(String.Empty))
      {
        IWindsorContainer container = migratorContainerFactory.CreateAndPopulateContainer(this);
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

    public Type ConnectionProviderType
    {
      get { return typeof(SqlServerConnectionProvider); }
    }

    public Type SchemaProviderType
    {
      get { return typeof(SqlServerSchemaProvider); }
    }

    public Type DatabaseProviderType
    {
      get { return typeof(SqlServerDatabaseProvider); }
    }
    #endregion
  }
}
