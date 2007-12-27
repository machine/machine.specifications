using System;
using System.Collections.Generic;

namespace Machine.Migrations.SchemaProviders
{
  public interface ISchemaProvider
  {
    void AddTable(string table, ICollection<Column> columns);
    void DropTable(string table);
    void RenameTable(string table, string newName);
    void AddColumn(string table, string column, Type type, bool isPrimaryKey);
    void RemoveColumn(string table, string column);
    void RenameColumn(string table, string column, string newName);
    bool HasTable(string table);
    bool HasColumn(string table, string column);
  }
}
