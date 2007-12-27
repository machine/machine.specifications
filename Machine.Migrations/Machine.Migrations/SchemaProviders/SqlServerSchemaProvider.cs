using System;
using System.Collections.Generic;
using System.Text;

using Machine.Migrations.DatabaseProviders;

namespace Machine.Migrations.SchemaProviders
{
  public class SqlServerSchemaProvider : ISchemaProvider
  {
    #region Member Data
    private readonly IDatabaseProvider _databaseProvider;
    #endregion

    #region SqlServerSchemaProvider()
    public SqlServerSchemaProvider(IDatabaseProvider databaseProvider)
    {
      _databaseProvider = databaseProvider;
    }
    #endregion

    #region ISchemaProvider Members
    public void AddTable(string table, ICollection<Column> columns)
    {
      if (columns.Count == 0)
      {
        throw new ArgumentException("columns");
      }
      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push("AddTable"))
      {
        StringBuilder sb = new StringBuilder();
        sb.Append("CREATE TABLE ").Append(table).Append(" (");
        bool first = true;
        foreach (Column column in columns)
        {
          if (!first) sb.Append(",");
          sb.AppendLine().Append(ColumnToCreateTableSql(column).Trim());
          first = false;
        }
        sb.AppendLine().Append(")");
        _databaseProvider.ExecuteNonQuery(sb.ToString());
      }
    }

    public void DropTable(string table)
    {
      _databaseProvider.ExecuteNonQuery("DROP TABLE {0}", table);
    }

    public bool HasTable(string table)
    {
      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push("HasTable({0})", table))
      {
        return _databaseProvider.ExecuteScalar<Int32>("SELECT COUNT(*) FROM syscolumns WHERE id = object_id('{0}')", table) > 0;
      }
    }

    public void AddColumn(string table, string column, Type type, bool isPrimaryKey)
    {
      _databaseProvider.ExecuteNonQuery("ALTER TABLE {0} ADD COLUMN {1}", table, ColumnToCreateTableSql(new Column(column, type, 0, isPrimaryKey)));
    }

    public void RemoveColumn(string table, string column)
    {
      _databaseProvider.ExecuteNonQuery("ALTER TABLE {0} DROP COLUMN {1}", table, column);
    }

    public void RenameTable(string table, string newName)
    {
      _databaseProvider.ExecuteNonQuery("EXEC sp_rename '{0}', '{1}'", table, newName);
    }

    public void RenameColumn(string table, string column, string newName)
    {
      _databaseProvider.ExecuteNonQuery("EXEC sp_rename '{0}.{1}', '{2}', 'COLUMN'", table, column, newName);
    }

    public bool HasColumn(string table, string column)
    {
      using (Machine.Core.LoggingUtilities.Log4NetNdc.Push("HasColumn({0}.{1})", table, column))
      {
        if (!HasTable(table))
        {
          return false;
        }
        return _databaseProvider.ExecuteScalar<Int32>("SELECT COUNT(*) FROM syscolumns WHERE id = object_id('{0}') AND name = '{1}'", table, column) > 0;
      }
    }
    #endregion

    #region Member Data
    public static string ColumnToCreateTableSql(Column column)
    {
      return String.Format("{0} {1} {2} {3}", column.Name, DotNetToSqlType(column.Type), column.AllowNull ? "" : "NOT NULL", column.IsPrimaryKey ? "PRIMARY KEY" : "");
    }

    public static string DotNetToSqlType(Type type)
    {
      if (type == typeof(Int16))
      {
        return "INT";
      }
      if (type == typeof(Int32))
      {
        return "INT";
      }
      if (type == typeof(Int64))
      {
        return "BIGINT";
      }
      if (type == typeof(String))
      {
        return "NVARCHAR(MAX)";
      }
			if (type == typeof(DateTime))
			{
				return "DATETIME";
			}
			if (type == typeof(bool))
			{
				return "BIT";
			}
			if (type == typeof(float) || type == typeof(double))
			{
				return "REAL";
			}
      throw new ArgumentException("type");
    }
    #endregion
  }
}
