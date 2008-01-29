using System;
using System.Collections.Generic;
using Machine.Migrations.DatabaseProviders;
using Machine.Migrations.SchemaProviders;

namespace Machine.Migrations.Services.Impl
{
  public class CommonTransformations : ICommonTransformations
  {
    #region Member Data
    private readonly IDatabaseProvider _databaseProvider;
    private readonly ISchemaProvider _schemaProvider;
    #endregion

    #region CommonTransformations()
    public CommonTransformations(IDatabaseProvider databaseProvider, ISchemaProvider schemaProvider)
    {
      _databaseProvider = databaseProvider;
      _schemaProvider = schemaProvider;
    } 
    #endregion

    #region ICommonTransformations Members
    public void AddColumn(string table, string column, Type type, short size, bool allowNull, object currentValue)
    {
      _schemaProvider.AddColumn(table, column, type, size, false, true);
      if (currentValue != null)
      {
        _databaseProvider.ExecuteNonQuery("UPDATE {0} SET {1} = '{2}'", table, column, currentValue);
      }
      if (!allowNull)
      {
        _schemaProvider.ChangeColumn(table, column, type, 0, allowNull);
      }
    }
    #endregion
  }
}
