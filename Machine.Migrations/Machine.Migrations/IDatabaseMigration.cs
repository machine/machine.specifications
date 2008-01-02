using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;
using Machine.Migrations.Services;

namespace Machine.Migrations
{
  public interface IDatabaseMigration
  {
    void Initialize(IConfiguration configuration, IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider);
    void Up();
    void Down();
  }
}
