using System;
using System.Collections.Generic;

using Machine.Migrations.DatabaseProviders;

namespace Machine.Migrations.SchemaProviders
{
  public class SqlServerCeSchemaProvider : SqlServerSchemaProvider
  {
    #region SqlServerCeSchemaProvider()
    public SqlServerCeSchemaProvider(IDatabaseProvider databaseProvider)
     : base(databaseProvider)
    {
    }
    #endregion

    #region ISchemaProvider Members
    public override string ColumnToConstraintsSql(string tableName, Column column)
    {
      if (column.IsPrimaryKey)
      {
        return String.Format("CONSTRAINT PK_{0} PRIMARY KEY (\"{1}\")", tableName, column.Name);
      }
      return null;
    }

    public override string DotNetToSqlType(Type type, int size)
    {
      if (type == typeof(byte[]))
      {
        return "IMAGE";
      }
      if (type == typeof(String))
      {
        if (size == 0)
        {
          return "NTEXT";
        }
        return String.Format("NVARCHAR({0})", size);
      }
      return base.DotNetToSqlType(type, size);
    }
    #endregion
  }
}
