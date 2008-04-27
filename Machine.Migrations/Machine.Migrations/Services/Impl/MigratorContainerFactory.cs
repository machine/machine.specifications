using System;
using System.Collections.Generic;

using Machine.Container;
using Machine.Container.Services;
using Machine.Core.Services;
using Machine.Core.Services.Impl;
using Machine.WindsorExtensions;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class MigratorContainerFactory : IMigratorContainerFactory
  {
    #region IMigratorContainerFactory Members
    public virtual IHighLevelContainer CreateAndPopulateContainer(IConfiguration configuration)
    {
      IHighLevelContainer container = CreateContainer();
      container.Initialize();
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
      container.AddService<IConfiguration>(configuration);
      container.AddService<CSharpMigrationFactory>();
      container.AddService<BooMigrationFactory>();
      return container;
    }
    #endregion

    public virtual IHighLevelContainer CreateContainer()
    {
      return new MachineContainer();
    }
  }
}
