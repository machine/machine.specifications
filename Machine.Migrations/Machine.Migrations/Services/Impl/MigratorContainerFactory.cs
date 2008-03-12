using System;
using System.Collections.Generic;

using Castle.Windsor;

using Machine.Core.Services;
using Machine.Core.Services.Impl;
using Machine.IoC;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class MigratorContainerFactory : IMigratorContainerFactory
  {
    #region IMigratorContainerFactory Members
    public virtual IWindsorContainer CreateAndPopulateContainer(IConfiguration configuration)
    {
      IWindsorContainer windsor = CreateContainer();
      WindsorWrapper container = new WindsorWrapper(windsor);
      container.AddService<IConnectionProvider>(configuration.ConnectionProviderType);
      container.AddService<ITransactionProvider, TransactionProvider>();
      container.AddService<IDatabaseProvider>(configuration.DatabaseProviderType);
      container.AddService<ISchemaProvider>(configuration.SchemaProviderType);
      container.AddService<IFileSystem, FileSystem>();
      container.AddService<INamer, Namer>();
      container.AddService<ISchemaStateManager, SchemaStateManager>();
      container.AddService<IMigrationFinder, MigrationFinder>();
      container.AddService<IMigrationSelector, MigrationSelector>();
      container.AddService<IMigrationRunner, MigrationRunner>();
      container.AddService<IMigrationInitializer, MigrationInitializer>();
      container.AddService<IMigrator, Migrator>();
      container.AddService<IMigrationFactoryChooser, MigrationFactoryChooser>();
      container.AddService<IVersionStateFactory, VersionStateFactory>();
      container.AddService<IWorkingDirectoryManager, WorkingDirectoryManager>();
      container.AddService<ICommonTransformations, CommonTransformations>();
      container.AddService(configuration);
      container.AddService<CSharpMigrationFactory>();
      container.AddService<BooMigrationFactory>();
      return windsor;
    }
    #endregion

    public virtual IWindsorContainer CreateContainer()
    {
      return new WindsorContainer();
    }
  }
}
