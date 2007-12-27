using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations
{
  public interface IDatabaseMigration
  {
    void Initialize(IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider);
    void Up();
    void Down();
  }
}
