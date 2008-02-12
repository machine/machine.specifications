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

    public IDisposable EnableIdentityInsertion(string table)
    {
      SetIdentityInsertion(table, true);
      return new DisableIdentityInsertion(this, table);
    }

    public void SetIdentityInsertion(string table, bool enabled)
    {
      _databaseProvider.ExecuteNonQuery("SET IDENTITY_INSERT \"{0}\" {1}", table, enabled ? "ON" : "OFF");
    }
    #endregion
  }
  public class DisableIdentityInsertion : IDisposable
  {
    private readonly ICommonTransformations _commonTransformations;
    private readonly string _table;

    public DisableIdentityInsertion(ICommonTransformations commonTransformations, string table)
    {
      _commonTransformations = commonTransformations;
      _table = table;
    }

    public void Dispose()
    {
      _commonTransformations.SetIdentityInsertion(_table, false);
    }
  }
}
